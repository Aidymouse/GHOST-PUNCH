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
	/** Ghost is ragdolled */
	RAGDOLL,
	/** Unused ? */
	RECOVERY,
	/** When the ghost is crawling around */
	SCURRY,
	// The state of the ghost rising from the ground - could be wrapped into ragdoll?
	GET_UP,

	STAGGER_MINOR,
	STAGGER_MEDIUM,
	STAGGER_LARGE,

	/** @deprecated */
	TWITCH,

	// Power actions
	POW_SLAP,
	POW_BLAST,

	POW_RUSH,
	POW_SCREAM,

	POW_CHARGING_ESCAPE,

	/** To categorise */
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

	[HideInInspector] public float escape_meter;
	[HideInInspector] public float escape_needed;

  // Hit points
  [HideInInspector]
  public float hp;

  public GameObject ghostPuncher_obj;
  [HideInInspector] public GhostPuncher ghostPuncher;

  /** Used to find colliders and rigidbodies for switching between ragdoll and animator */
  public GameObject rig;
  [HideInInspector] public GameObject nav_destination;
  [HideInInspector] public ParticleSystem charge_particles;
  Animator anim;

	/** Position the ghost currently wants to look towards */
	Vector3? look_at;
	/** If present, the ghost will update it's look_at vector to be the position of this target */
	GameObject? look_target;

	[Header("Ragdoll")]
  [HideInInspector]
	public RagdollAnimator ragdoll_animator;
	public RagdollSettings ragdoll_settings;
	public RagdollPowerProfile ragprof_animated;
	public RagdollPowerProfile ragprof_doll;
	// The ragdoll object itself, moved when the ghost is getting up
	public GameObject ragdoll;

	[Header("Knockback")]
	public Rigidbody kb_rigidbody;
	public Collider kb_collider;
	Timer ti_knockback;
	// TODO: the ghost sinks into the floor a little? Collider needs to accomodate that


  [Header("Sound Effects")]
	public GhostSounds ghost_sfx;
  public AudioSource ghost_sound;
	// Seperate audio sources because we need different generators for each fucking varied thing!!!
	// It's okay the ghost is worth it
  public AudioSource ghost_screams;
	[Tooltip("Ghost hit sound and particles determined by this material")]
	public ObjectMaterial ghost_material;

  public float pitchLow;
  public float pitchHigh;

  public GhostActions cur_action;
	// Interrupt action takes precdence, allowing staggers to interrupt actions
	public GhostActions? interrupt_action = null;
  public GhostAction[] actions;

  // jumpscare sequence
  [Header("Jumpscare")]
  public bool jumpscareReady = false;
  public float jumpscareDistance = 2.0f;
  public Transform jumpscareAlignPoint;
  public Transform jumpscareTarget;
  public PlayableDirector jumpscareTimeline;
  public bool inJumpscare = false;

  public float fear_meter;

  // Spawns when the ghost uses her wave power
  public GameObject wave_orb;

  public NavMeshAgent nav_agent;

	// TODO: put in respective states
  [HideInInspector] public Timer ti_hit_stun;
  [HideInInspector] public Timer ti_ragdoll;
  [HideInInspector] public Timer ti_restore_poise;

  // When poise hit's 0, the ghost staggers, which makes her vulnerable.
  [HideInInspector] public float poise;
  [HideInInspector] public float max_poise;

  Rigidbody[] rig_rbs;
  Collider[] rig_colliders;
  CharacterJoint[] rig_joints;

	/** Actions */
	public GhostActionSet close_range_actions;
	public GhostActionSet mid_range_actions;
	public GhostActionSet far_range_actions;

	void Awake() {
    ghostPuncher = ghostPuncher_obj.GetComponent<GhostPuncher>();

		LookAt(ghostPuncher.gameObject);

		// Apply Defaults
    poise = defaults.POISE;
		max_poise = defaults.POISE;
    hp = defaults.HP;
		escape_needed = defaults.ESCAPE_NEEDED;
		escape_meter = 0;

		ti_knockback = new Timer(0, defaults.KNOCKBACK_TIMER);
	}

  void Start()
  {
		ragdoll_animator = GetComponentInChildren<RagdollAnimator>();
	ragdoll_animator.forceTargetPose = true;

    /* Nav Settings */
    //nav_agent = GetComponent<NavMeshAgent>();
    nav_agent.updateRotation = false;

    /* Init Actions */
    actions = new GhostAction[20];
    actions[(int)GhostActions.STAGGER_LARGE] = new GA_StaggerLarge(this);
    actions[(int)GhostActions.STAGGER_MEDIUM] = new GA_StaggerMedium(this);
    actions[(int)GhostActions.RAGDOLL] = new GA_Ragdoll(this);
    actions[(int)GhostActions.RECOVERY] = new GA_Recovery(this);
    actions[(int)GhostActions.GET_UP] = new GA_GetUp(this, ragdoll_animator.MasterAlpha);
    actions[(int)GhostActions.SCURRY] = new GA_Scurry(this);

		// Power Actions
    actions[(int)GhostActions.POW_CHARGING_ESCAPE] = new GA_POW_ChargingEscape(this);
    actions[(int)GhostActions.POW_SLAP] = new GA_POW_Slap(this);
    actions[(int)GhostActions.POW_RUSH] = new GA_Rush(this);
    actions[(int)GhostActions.POW_BLAST] = new GA_Blast(this);

    rig_rbs = rig.GetComponentsInChildren<Rigidbody>();
    rig_colliders = rig.GetComponentsInChildren<Collider>();
    rig_joints = rig.GetComponentsInChildren<CharacterJoint>();

    DisableRagdoll();

    // needs to update on ghost slap somehow too		

    fear_meter = 0;



    /* Timers */
    ti_restore_poise = new Timer(0, defaults.POISE_RESTORE_TIMER);
    ti_ragdoll = new Timer(0, defaults.RAGDOLL_TIME);

    /* Animator */
    anim = this.GetComponentInChildren<Animator>();

    charge_particles = GetComponentInChildren<ParticleSystem>();


		if (debug.ghost_stay_awake) {
			return;
		}
		
		// The ghost will go to sleep!
		GoDormant();

  }

  // Update is called once per frame
  public void Update()
  {

		if (!ti_knockback.Finished()) {
			ti_knockback.Tick(Time.deltaTime);
			if (ti_knockback.Finished()) {
				DisableKnockback();
			}
		}

    /* Update where ghost is looking */
		// Unless otherwise specified, use the ghost punchers location
		
		
    if (!SpinDisabled() && (look_at is not null || look_target is not null)) {
			if (look_target is not null) {
				look_at = look_target.transform.position;
			}
			Vector3 look_pos = look_at.Value;
      Vector3 toLookedAt = look_pos - transform.position;
      toLookedAt.y = 0;
			if (toLookedAt != Vector3.zero) {
				Quaternion lookedAngle = Quaternion.LookRotation(toLookedAt);
				float turn_speed = nav_agent.angularSpeed;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, lookedAngle, turn_speed * Time.deltaTime);
			}
			//Debug.DrawRay(transform.position, toLookedAt*100, Color.cyan);
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
			if (IsVulnerable()) {
				StopBeingVulnerable();
			}
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

	public void ExitAction(GhostActions? next_action = null) {
		if (debug.ghost_logs) Debug.Log("Ghost Exiting: "+cur_action);
		actions[(int)cur_action].Exit();

		// TODO: will this always be the case?
		if (next_action is not null) {
			EnterAction(next_action.Value);
		} else {
			DecideNextAction();
		}
  }

  public void EnterAction(GhostActions action) {
		cur_action = action;
		if (debug.ghost_logs) Debug.Log("Ghost Entering: "+cur_action);
		actions[(int)cur_action].Enter();
  }

	void DecideNextAction() {
		// How far am I from ghost puncher?
		Vector3 to_ghost_puncher = transform.position - ghostPuncher.transform.position;

		GhostActionSet choose_from = close_range_actions;

		float max_height_diff = 2;
		if (to_ghost_puncher.y > max_height_diff) {
			if (debug.ghost_logs) Debug.Log("Choosing FAR range action");
			choose_from = far_range_actions;
		} else {
			to_ghost_puncher.y = 0;
			float dist = to_ghost_puncher.magnitude;

			if (dist > defaults.FAR_RANGE) {
				if (debug.ghost_logs) Debug.Log("Choosing FAR range action");
				choose_from = far_range_actions;
			} else if (dist > defaults.MID_RANGE) {
				if (debug.ghost_logs) Debug.Log("Choosing MID range action");
				choose_from = mid_range_actions;
			} else {
				if (debug.ghost_logs) Debug.Log("Choosing CLOSE range action");
			}
		}

		if (choose_from.debug) {
			EnterAction(choose_from.debug_action);
			return;
		}

		EnterAction(choose_from.WeightedPickAction());
	}


  void TickTimers() {
    if (cur_action != GhostActions.STAGGER_LARGE) {
      ti_restore_poise.Tick(Time.deltaTime);
    }

  }



  /** EVENTS **/
	public void GetPunched(Punch punch, RaycastHit? hit=null) {

		// Damage
		hp -= punch.ghost_damage;

		// Particles
		if (ghost_material) {
			if (ghost_material.hit_sound) {
				SoundEmitter.PlayVariedSoundAtPoint(ghost_material.hit_sound, transform.position, ghost_material.pitch_low, ghost_material.pitch_high);
			}

			if (ghost_material.hit_particles && hit is not null) {
				ParticleSystem p = Instantiate(ghost_material.hit_particles);
				p.transform.position = hit.Value.point;
				p.transform.rotation = Quaternion.Euler(-hit.Value.normal);

			}
		}

		actions[(int)cur_action].GetPunched(punch, hit);


		// 1 is mega punch and 3 is big object hit
		if (IsVulnerable() && (punch.hit_class <= (int)HitClass.LARGE_ITEM)) {
			Ragdoll(punch);
			return;
		}

		if (punch.hit_class <= (int)HitClass.PUNCH) {
			ghost_screams.Play();
			ghost_sound.PlayOneShot(ghost_sfx.HIT_SOUND);
		}

		// Poise
		poise -= punch.poise_damage;
		if (poise <= 0) {
			if (punch.hit_class <= (int)HitClass.MEGA_PUNCH) {
				Ragdoll(punch);

			} else if (poise + punch.poise_damage > 0) {
				// Only happens if this hit got us below 0
				BecomeVulnerable();
				ExitAction(GhostActions.STAGGER_LARGE);
			}
		} else {
			ti_restore_poise.Reset();
			// TODO: minor stagger
		}


		if (HasHyperArmor()) {
			// TODO: some minor jolts, but no damage
			return;
		}

		PlayMinorHurtAnim();

		if (cur_action == GhostActions.RAGDOLL) {
			// TODO: special punch case when down
			return;
		}

		// Knockback
		// TODO: look into nav_agent.nextPosition as possible point
		/*
			 EnableKnockback();
			 Vector3 knockback_dir = punch.direction;
			 knockback_dir.y = 0;
			 kb_rigidbody.AddForce(knockback_dir.normalized * punch.force * defaults.KNOCKBACK_FACTOR);
			 ti_knockback.Reset();
			 */


	}

	// Layer anim
	void PlayMinorHurtAnim() {
		int hurt_anim = Random.Range(1,2+1);
		ChangeAnimation("Hurt"+hurt_anim);
	}

  void GainFear(int fear_gained) {
    fear_meter += fear_gained;
  }

  void LoseFear(int fear_lost) {
    fear_meter -= fear_lost;
  }

  public void RestorePoise() {
		if (poise >= max_poise) { return; }
    poise = max_poise;
	
  }

  void BecomeVulnerable() {
		GainFear(5); // needed?
  }

  public void StopBeingVulnerable() {
		RestorePoise();
  }


	public void Stagger(GhostActions stagger_action, Punch punch) {
		int stagger_level = (int)stagger_action;
		if (IsVulnerable()) { stagger_level += 1; }
		
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
    ghost_sound.PlayOneShot(ghost_sfx.RAGDOLL_SCREAM);

		ExitAction(GhostActions.RAGDOLL);

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

		// TODO: reset health and shit!

		this.GetComponent<Ghost>().enabled = true;
		DecideNextAction();
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
		shop_master.CurryStartEndRun();
	}

	// Called from GHOSTPUNCH
	public void EndRun() {
		// TODO:
		GoDormant();
	}

	public void GoDormant() {
		this.GetComponent<Ghost>().enabled = false;
	}

	public void PlaySound(string clip_name) {

	}


	public void EnableKnockback() {
		nav_agent.isStopped = true;

		kb_rigidbody.isKinematic = false;
		kb_collider.enabled = true;
	}
	
	public void DisableKnockback() {
		nav_agent.isStopped = false;

		kb_rigidbody.isKinematic = true;
		kb_collider.enabled = false;
	}

  /** GETTERS */
  public NavMeshAgent get_nav_agent() { return nav_agent; }

	public bool IsVulnerable() { return this.poise <= 0; }

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

	/** Looks at static vector 3 location, overriding look target */
	public void LookAt(Vector3 look_pos) {
		look_target = null;
		look_at = look_pos;
	}

	/** Makes game object the focus of the ghosts attention, updated look_at to always focus on this things transform.position */
	public void LookAt(GameObject new_target) {
		look_target = new_target;
		look_at = look_target.transform.position;
	}

	/** Ghost will not face any particular direction */
	public void StopLooking() {
		look_at = null;
		look_target = null;
	}
}
