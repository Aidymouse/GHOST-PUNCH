using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Unity.Properties;
using UnityEngine.InputSystem;

using UnityEditor;

public enum GhostActions {
  CHARGING_ESCAPE,
  STARTLED, // TODO
  MOVING_ROOM,
  HIT_STUN,
  RAGDOLL,
  RECOVERY,

  USING_POWER,

	PEAK_AROUND_CORNER,
};

public class Ghost : MonoBehaviour
{

  public GhostDefaults defaults;
  public GhostPowerAttribs power_attribs;

	// Elevated permissions on this one. We need to be able to end runs!
	public GHOSTPUNCH game;

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

  [HideInInspector]
  public GameObject nav_destination;
  [HideInInspector]
  public ParticleSystem charge_particles;
  Animator anim;


  [Header("Sound Effects")]
  public AudioSource currentSound;
  public AudioClip takingDamageSound;
  public AudioClip ragdollSound;
  //Sounds temporarily stored on objects lol
  public AudioClip energySound;
  public AudioClip jumpscareSound;
  public float pitchLow;
  public float pitchHigh;



  public GhostActions cur_action;
  public GhostAction[] actions;

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

  GhostPower[] powers;
  GhostPower active_power;

  // Spawns when the ghost uses her wave power
  public GameObject wave_orb;

  [HideInInspector]
  public NavMeshAgent nav_agent;

  [HideInInspector]
  public Timer ti_hit_stun;
  [HideInInspector]
  public Timer ti_ragdoll;
  [HideInInspector]
  public Timer ti_restore_poise;
  [HideInInspector]
  public Timer ti_recovery;

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

    /* Init Actions */
    actions = new GhostAction[7];
    actions[(int)GhostActions.CHARGING_ESCAPE] = new GhostAction_ChargingEscape(this);
    actions[(int)GhostActions.MOVING_ROOM] = new GhostAction_MovingRoom(this);
    actions[(int)GhostActions.HIT_STUN] = new GhostAction_HitStun(this);
    actions[(int)GhostActions.RAGDOLL] = new GhostAction_Ragdoll(this);
    actions[(int)GhostActions.RECOVERY] = new GhostAction_Recovery(this);


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

    /* Nav Settings */
    nav_agent = GetComponent<NavMeshAgent>();
    nav_agent.updateRotation = false;
    //nav_agent.destination = nav_destination.position;

    charge_particles = GetComponentInChildren<ParticleSystem>();

    //nav_destination = null;

    /* Powers */
    // Set up last so any objects retrieved in constructors are present
    powers = new GhostPower[3];
    powers[0] = new GhostPower_Wave(this, power_attribs);
    powers[1] = new GhostPower_Slap(this, power_attribs);
    powers[2] = new GhostPower_Scream(this, power_attribs);

    EnterAction(GhostActions.MOVING_ROOM);

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
    if (to_target.magnitude > 0) {
      Quaternion target_angle = Quaternion.LookRotation(to_target);
      nav_agent.transform.rotation = target_angle;
    }
    //transform.TurnTowards(ghostPuncher.transform);

    if (ti_restore_poise.finished_this_frame()) {
      RestorePoise();
    }

    /* Actions */
		bool escaped_yet = Escaped();

    switch (cur_action) {
      case GhostActions.CHARGING_ESCAPE: 
      case GhostActions.MOVING_ROOM: 
      case GhostActions.HIT_STUN: 
      case GhostActions.RAGDOLL: 
      case GhostActions.RECOVERY: 
				actions[(int)cur_action].Update(); 
				break;

      case GhostActions.USING_POWER: 
				// TODO: i'm not sure how to refactor this.
				state_UsingPower();
				break;

    }

    tick_timers();

		if (!escaped_yet && Escaped()) {
			CallEndRun();
		}

  }


	void ExitAction() {
    // Logic based on what state we're leaving
		switch (cur_action) {
			case GhostActions.CHARGING_ESCAPE: 
				actions[(int)GhostActions.CHARGING_ESCAPE].Exit();
				break;

			case GhostActions.RAGDOLL:
				actions[(int)GhostActions.RAGDOLL].Exit();
				break;

    };
  }

  public void EnterAction(GhostActions action) {

    ExitAction();


    // Enter New State Logic
    switch (action) {
      case GhostActions.CHARGING_ESCAPE: 
      case GhostActions.MOVING_ROOM: 
      case GhostActions.HIT_STUN: 
      case GhostActions.RAGDOLL:
      case GhostActions.RECOVERY:
				actions[(int)action].Enter();
				cur_action = action;
				break;

      case GhostActions.USING_POWER: 
				cur_action = action;
				PickRandomPower();
				break;

    }

  }

  /** STATES **/

  void state_UsingPower() {
    active_power.Update();

    if (active_power.phase == GhostPower.GhostPowerPhase.DONE) {
      LeavePower();
    }
  }

  /**** POWERS ****/
  void PickRandomPower() {
    int power_index = power_attribs.OVERRIDE_POWER_IDX == -1 ? Random.Range(0, powers.Length) : power_attribs.OVERRIDE_POWER_IDX; 
    active_power = powers[power_index];
    active_power.Reset();
    active_power.Start();
  }

  void LeavePower() {
    active_power.End();

    if (Random.Range(1,4) == 3) {
      PickRandomPower();
    } else {
      EnterAction(GhostActions.MOVING_ROOM);
    }
  }

  void tick_timers() {
    if (cur_action != GhostActions.HIT_STUN) {
      ti_restore_poise.tick(Time.deltaTime);
    }
  }



  /** EVENTS **/
  public void GetPunched(Punch punch) {

    hp -= punch.GhostDamage;
    fear_meter += punch.Fear;

    currentSound.clip = takingDamageSound;
    currentSound.pitch = (Random.Range(pitchLow, pitchHigh));
    currentSound.Play();


    // 1 is mega punch and 3 is big object hit
    if (vulnerable && (punch.HitClass == 1 || punch.HitClass == 3)) {

      Ragdoll(punch);
      return;
    }


    if (HasHyperArmor()) {
      return;
    }

    poise -= punch.PoiseDamage;

    if (ectoplasm_particles) {
      Instantiate(ectoplasm_particles, transform.position, new Quaternion());
    }

    if (poise <= 0) {
      if (punch.HitClass <= 1) {
				Ragdoll(punch);

      } else {
				BecomeVulnerable();
				EnterAction(GhostActions.HIT_STUN);
      }
    } else {
      ti_restore_poise.reset();

      // TODO: play a random hit animation
      //int hurt_num = Random.Range(1,3);
      //PlayAnimation("Hurt"+hurt_num);
      PlayAnimation("Hurt1");

      if (cur_action == GhostActions.CHARGING_ESCAPE) {
				// TODO: there should be a bit of buffer time here or powers come out super fast
				EnterAction(GhostActions.USING_POWER);
      }
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
    fear_meter += 5;
  }

  void StopBeingVulnerable() {
    vulnerable = false;
  }

  void Ragdoll(Punch punch) {
    currentSound.clip = ragdollSound;
    currentSound.PlayOneShot(ragdollSound);
    currentSound.Play();
    EnterAction(GhostActions.RAGDOLL);
    rig_core.AddForce(punch.Direction * punch.Force * defaults.MAKE_HER_FLY_FACTOR);
  }

  /** STATUS **/
  // If the ghost has hyper armor, she cannot have her poise break (it can go down though)
  bool HasHyperArmor() {
    return cur_action == GhostActions.HIT_STUN || cur_action == GhostActions.RAGDOLL || cur_action == GhostActions.RECOVERY;

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
    DisableRagdoll();
    anim.enabled = true;
  }

  public void DisableAnimator() {
    anim.enabled = false;
  }

  public void PlayAnimation(string new_anim) {
    //anim.Rewind(new_anim);
    anim.Play(new_anim, -1, 0.0f);
  }

  public void ChangeAnimation(string new_anim, float fade_time=0f) {
    anim.CrossFade(new_anim, fade_time);
  }

  public void EnableRagdoll() {
    DisableAnimator();
    /*foreach (Collider col in rig_colliders) {
      col.enabled = true;
      }*/
    foreach (Rigidbody rb in rig_rbs) {
      rb.detectCollisions = true;
      rb.useGravity = true;
      rb.isKinematic = false;
    }
    foreach (CharacterJoint joint in rig_joints) {
      joint.enableCollision = true;
    }
  }

  public void DisableRagdoll() {
    /*foreach (Collider col in rig_colliders) {
      col.enabled = false;
      }*/
    foreach (Rigidbody rb in rig_rbs) {
      //rb.detectCollisions = false;
      rb.useGravity = false;
      rb.isKinematic = true;
    }
    foreach (CharacterJoint joint in rig_joints) {
      joint.enableCollision = false;
    }
  }


	public void StartRun() {
		gameObject.SetActive(true);
	}

	/* Just pass through to game */
	public void CallEndRun() {
			game.EndRun();
	}

	// Called from GHOSTPUNCH
	public void EndRun() {
		// TODO:
	}



  /** GETTERS */

  public NavMeshAgent get_nav_agent() { return nav_agent; }
}
