using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Unity.Properties;
using UnityEngine.InputSystem;
using Hairibar.Ragdoll.Animation;
using Hairibar.Ragdoll;
using System.Collections.Generic;

public enum GhostActions {
	// Non-power actions (states)
	STARTLED, // TODO
	RAGDOLL,
	RECOVERY,
	// The state of the ghost rising from the ground - could be wrapped into ragdoll?
	GET_UP,

	STAGGER_MINOR,
	STAGGER_MEDIUM,
	STAGGER_LARGE,

	// Power actions
	POW_CHARGING_ESCAPE,
	POW_SLAP,
	POW_SCREAM,
	POW_BLAST,
	POW_JUMPSCARE,
	POW_PEAK_AROUND_CORNER,
};


public class Ghost : MonoBehaviour
{

  public GhostDefaults defaults;
	public GPDebug debug;

	// Elevated permissions on this one. We need to be able to end runs!
	public ShopMaster shop_master;

  // Forces are applied to the rig core to send the ghost flying
  public Rigidbody rig_core;

	[HideInInspector]
  public float escape_meter;
	[HideInInspector]
  public float escape_needed;

  // Hit points
  [HideInInspector]
  public float hp;

  public GameObject ghostPuncher_obj;
  [HideInInspector]
  public GhostPuncher ghostPuncher;

  /** Used to find colliders and rigidbodies for switching between ragdoll and animator */
  public GameObject rig;
  [HideInInspector] public GameObject nav_destination;
  [HideInInspector] public ParticleSystem charge_particles;
  Animator anim;

	[Header("Ragdoll")]
  [HideInInspector]
	public RagdollAnimator ragdoll_animator;
	public RagdollSettings ragdoll_settings;
	public RagdollPowerProfile ragprof_animated;
	public RagdollPowerProfile ragprof_doll;
	// The ragdoll object itself, moved when the ghost is getting up
	public GameObject ragdoll;


  [Header("Sound Effects")]
  public AudioSource currentSound;
  public AudioClip takingDamageSound;
  public AudioClip ragdollSound;
  //Sounds temporarily stored on objects lol
  public AudioClip energySound;
  public AudioClip jumpscareSound;
	public AudioClip sfx_charging_escape;

	[SerializeField]
	public Dictionary<string, AudioClip> sfx;

  public float pitchLow;
  public float pitchHigh;



  public GhostActions cur_action;
	// Interrupt action takes precdence, allowing staggers to interrupt actions
	public GhostActions? interrupt_action = null;
  public GhostAction[] actions;
	GhostActions[] powers = { 
		GhostActions.POW_CHARGING_ESCAPE,
		GhostActions.POW_SLAP,
	};

  // jumpscare sequence
  [Header("Jumpscare")]
  public bool jumpscareReady = false;
  public float jumpscareDistance = 2.0f;
  public Transform jumpscareAlignPoint;
  public Transform jumpscareTarget;
  public PlayableDirector jumpscareTimeline;
  public bool inJumpscare = false;

  public ParticleSystem ectoplasm_particles;

  public float fear_meter;

  // Spawns when the ghost uses her wave power
  public GameObject wave_orb;

  [HideInInspector] public NavMeshAgent nav_agent;

	// TODO: put in respective states
  [HideInInspector] public Timer ti_hit_stun;
  [HideInInspector] public Timer ti_ragdoll;
  [HideInInspector] public Timer ti_restore_poise;
  [HideInInspector] public Timer ti_recovery;

  // When poise hit's 0, the ghost staggers, which makes her vulnerable.
  [HideInInspector]
  public float poise;
  [HideInInspector]
  public float max_poise;
  // If the ghost is vulnerable, a mega punch will send her flying
  [HideInInspector]
  public bool vulnerable;

  Rigidbody[] rig_rbs;
  Collider[] rig_colliders;
  CharacterJoint[] rig_joints;

  float turn_speed;

	void Awake() {
    ghostPuncher = ghostPuncher_obj.GetComponent<GhostPuncher>();

		// Apply Defaults
    poise = defaults.POISE;
		max_poise = defaults.POISE;
    hp = defaults.HP;
		escape_needed = defaults.ESCAPE_NEEDED;
		escape_meter = 0;
	}

  void Start()
  {
		ragdoll_animator = GetComponentInChildren<RagdollAnimator>();

    /* Nav Settings */
    nav_agent = GetComponent<NavMeshAgent>();
    nav_agent.updateRotation = false;

    /* Init Actions */
    actions = new GhostAction[20];
    actions[(int)GhostActions.STAGGER_LARGE] = new GA_StaggerLarge(this);
    actions[(int)GhostActions.RAGDOLL] = new GA_Ragdoll(this);
    actions[(int)GhostActions.RECOVERY] = new GA_Recovery(this);
    actions[(int)GhostActions.GET_UP] = new GA_GetUp(this, ragdoll_animator.MasterAlpha);

		// Power Actions
    actions[(int)GhostActions.POW_CHARGING_ESCAPE] = new GA_POW_ChargingEscape(this);
    actions[(int)GhostActions.POW_SLAP] = new GA_POW_Slap(this);



    rig_rbs = rig.GetComponentsInChildren<Rigidbody>();
    rig_colliders = rig.GetComponentsInChildren<Collider>();
    rig_joints = rig.GetComponentsInChildren<CharacterJoint>();

    DisableRagdoll();

    // needs to update on ghost slap somehow too		

    turn_speed = defaults.TURN_SPEED;
    fear_meter = 0;



    /* Timers */
    ti_hit_stun = new Timer(0, defaults.HIT_STUN_TIME);
    ti_restore_poise = new Timer(0, defaults.POISE_RESTORE_TIMER);
    ti_ragdoll = new Timer(0, defaults.RAGDOLL_TIME);
    ti_recovery = new Timer(0);

    /* Animator */
    anim = this.GetComponentInChildren<Animator>();

    //nav_agent.destination = nav_destination.position;

    charge_particles = GetComponentInChildren<ParticleSystem>();


		// Init - pick a random power to start doing
		PickRandomPower();

    currentSound = GetComponent<AudioSource>();
    currentSound.clip = takingDamageSound;

  }

  // Update is called once per frame
  public void Update()
  {

    /* Rotate towards ghost puncher */
    // TODO: when we flee we should look that direction instead
    if (!SpinDisabled()) {
      Vector3 toGhostPuncher = ghostPuncher.transform.position - transform.position;
      toGhostPuncher.y = 0;
      Quaternion ghostPuncher_angle = Quaternion.LookRotation(toGhostPuncher);
      float turn_speed = 100;
      transform.rotation = Quaternion.RotateTowards(transform.rotation, ghostPuncher_angle, turn_speed * Time.deltaTime);
    }


    /* Rotate nav agent always towards its next target (infinite turn speed ... doesn't work ?) */
    Vector3 to_target = nav_agent.steeringTarget - transform.position;
    to_target.y = 0;
	

		//Debug.Log(to_target.magnitude);
    if (to_target.x != 0 && to_target.y != 0 && to_target.z != 0) {
      Quaternion target_angle = Quaternion.LookRotation(to_target);
      nav_agent.transform.rotation = target_angle;
    }
    //transform.TurnTowards(ghostPuncher.transform);

    if (ti_restore_poise.FinishedThisFrame()) {
      RestorePoise();
    }

    /* Actions */
		bool escaped_yet = Escaped();

		if (interrupt_action is not null) {
		} else {
			actions[(int)cur_action].Update(); 
		}

    TickTimers();

		if (!escaped_yet && Escaped()) {
			CallEndRun();
		}

  }

	public void ExitAction() {
		actions[(int)cur_action].Exit();
		// TODO: will this always be the case?
		PickRandomPower();
  }

  public void EnterAction(GhostActions action) {
		cur_action = action;
		actions[(int)cur_action].Enter();
  }

  void PickRandomPower() {
		int power_index = Random.Range(0,powers.Length);
		if (debug.use_power_override) {
    	power_index = (int)debug.power_override;
		}

		EnterAction(powers[power_index]);
  }

  void TickTimers() {
    if (cur_action != GhostActions.STAGGER_LARGE) {
      ti_restore_poise.Tick(Time.deltaTime);
    }
  }



  /** EVENTS **/
  public void GetPunched(Punch punch) {

    hp -= punch.ghost_damage;

    currentSound.clip = takingDamageSound;
    currentSound.pitch = (Random.Range(pitchLow, pitchHigh));
    currentSound.Play();


    // 1 is mega punch and 3 is big object hit
    if (vulnerable && (punch.hit_class <= (int)HitClass.LARGE_ITEM)) {

      Ragdoll(punch);
      return;
    }


    if (HasHyperArmor()) {
			// TODO: some minor jolts, but no damage
      return;
    }

		if (cur_action == GhostActions.RAGDOLL) {
			// TODO: special punch case when down
			return;
		}

    poise -= punch.poise_damage;

    if (ectoplasm_particles) {
      Instantiate(ectoplasm_particles, transform.position, new Quaternion());
    }

    if (poise <= 0) {
      if (punch.hit_class <= (int)HitClass.MEGA_PUNCH) {
				Ragdoll(punch);

      } else {
				BecomeVulnerable();
				EnterAction(GhostActions.STAGGER_LARGE);
      }
    } else {
      ti_restore_poise.Reset();
			// TODO: minor stagger
    }
  }

  void GainFear(int fear_gained) {
    fear_meter += fear_gained;
  }

  void LoseFear(int fear_lost) {
    fear_meter -= fear_lost;
  }

  public void RestorePoise() {
    poise = max_poise;
  }

  void BecomeVulnerable() {
    vulnerable = true;
		GainFear(5); // needed?
  }

  void StopBeingVulnerable() {
    vulnerable = false;
  }

	public void Stagger(GhostActions stagger_action, Punch punch) {
		int stagger_level = (int)stagger_action;
		if (vulnerable) { stagger_level += 1; }
		
		if (stagger_level == (int)GhostActions.STAGGER_MINOR) {
			// TODO:
		} else if (stagger_level == (int)GhostActions.STAGGER_MEDIUM) {
			// TODO:
		} else if (stagger_level == (int)GhostActions.STAGGER_LARGE) {
			// TODO:
		} else if (stagger_level > (int)GhostActions.STAGGER_LARGE) {
			Ragdoll(punch);
		}
	}

  void Ragdoll(Punch punch) {
    currentSound.clip = ragdollSound;
    currentSound.PlayOneShot(ragdollSound);
    currentSound.Play();

    EnterAction(GhostActions.RAGDOLL);

    rig_core.AddForce(punch.direction * punch.force * defaults.MAKE_HER_FLY_FACTOR);
  }

	/* Enter a special power designated action */
	void PickPower() {
	}

  /** STATUS **/
  // If the ghost has hyper armor, she cannot have her poise break (it can go down though)
  bool HasHyperArmor() {
    return cur_action == GhostActions.STAGGER_LARGE || cur_action == GhostActions.RECOVERY;

  }

  bool SpinDisabled() {
    return cur_action == GhostActions.RAGDOLL;
  }

  public bool Escaped() {
    return escape_meter >= escape_needed;
  }

  /** RAGDOLL **/

  // TODO: wrap these in actual state changes so she doesn't keep trying to move around when she's ragdolled
  public void EnableAnimator() {
    ragdoll_animator.MasterAlpha = 1;
  }

  public void DisableAnimator() {
    ragdoll_animator.MasterAlpha = 0;
  }

  public void EnableRagdoll() { }
  public void DisableRagdoll() { }

	/** ANIMATION **/
  public void PlayAnimation(string new_anim) {
    //anim.Rewind(new_anim);
    anim.Play(new_anim, -1, 0.0f);
  }

  public void ChangeAnimation(string new_anim, float fade_time=0f) {
    anim.CrossFade(new_anim, fade_time);
  }

	/** CONTROL FNS **/
	public void StartRun() {
		escape_meter = 0;
		gameObject.SetActive(true);
		this.GetComponent<Ghost>().enabled = true;
	}

	public void ApplyItems(ItemRecord record) {
		for (int i=0; i<record.items.Count; i++) {
			Item item = record.items[i];
			item.ApplyToGhost(this);
		}
	}

	/* Just pass through to shop master */
	public void CallEndRun() {
		if (debug.dont_end_run == true || !shop_master) { return; }
		shop_master.EndRun();
	}

	// Called from GHOSTPUNCH
	public void EndRun() {
		// TODO:
		this.GetComponent<Ghost>().enabled = false;
	}

	public void PlaySound(string clip_name) {
		//currentSound.loop = false;	

		switch (clip_name) {
			case "charging_escape": {
				currentSound.clip = sfx_charging_escape;
				//currentSound.loop = true;	
				break;
			}
			default: {
				Debug.Log("Cannot play ghost sound: "+clip_name);
				break;
			}
		}
	}


  /** GETTERS */
  public NavMeshAgent get_nav_agent() { return nav_agent; }

	/** SETTERS */
	public void SetLayerInChildren(int layer, bool self_too = true) {

		if (self_too) {
			this.gameObject.layer = layer;
		}
		
		Transform[] children = GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform t in children) {
			t.gameObject.layer = layer;
		}
	}
}
