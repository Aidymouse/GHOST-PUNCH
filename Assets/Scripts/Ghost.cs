using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;

using UnityEditor;

public class Ghost : MonoBehaviour
{

  public GhostDefaults defaults;
  //public GhostUI UIData;
  //
  public GhostUI ui;
  public DebugUI debug_ui;
  public bool ragdoll;

  public float escape_meter;
  public float escape_needed;

  /** Used to find colliders and rigidbodies for switching between ragdoll and animator */
  public GameObject rig;

  GameObject nav_destination;
  ParticleSystem charge_particles;
  Animator anim;

  enum GhostAction {
    CHARGING_ESCAPE,
    STARTLED,
    HITSTUN,
    MOVING_ROOM,
    RECOVERY,
    HIT_STUN,
    RAGDOLL,

    POWER_WAVE,
  };


  GhostAction cur_action;

  NavMeshAgent nav_agent;

  Timer ti_hit_stun;
  // Reset every time the ghost gets hit. Resets hit stun resistance on finish
  Timer ti_hit_stun_reset;
  
  // Reduces hit stun and increases with each hit until reset
  float hit_stun_resistance;

  Rigidbody[] rig_rbs;
  Collider[] rig_colliders;
  CharacterJoint[] rig_joints;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {


    rig_rbs = rig.GetComponentsInChildren<Rigidbody>();
    rig_colliders = rig.GetComponentsInChildren<Collider>();
    rig_joints = rig.GetComponentsInChildren<CharacterJoint>();

    DisableRagdoll();

    ti_hit_stun = new Timer(0, defaults.HIT_STUN_TIME);
    ti_hit_stun_reset = new Timer(0, defaults.HIT_STUN_RESET_TIME);

    anim = this.GetComponentInChildren<Animator>();
    
    nav_agent = GetComponent<NavMeshAgent>();
    //nav_agent.destination = nav_destination.position;

    charge_particles = GetComponentInChildren<ParticleSystem>();

    //nav_destination = null;

    escape_meter = 0;
    escape_needed = 60;

    EnterAction(GhostAction.MOVING_ROOM);

  }

  // Update is called once per frame
  void Update()
  {

    if (!ti_hit_stun_reset.finished()) {
      ti_hit_stun_reset.tick(Time.deltaTime);
      if (ti_hit_stun_reset.finished()) {
	hit_stun_resistance = 0;
      }
    }

    switch (cur_action) {
      case GhostAction.CHARGING_ESCAPE: {
	debug_ui.SetDebug2("Ghost Action: Charging Escape");
	state_ChargingEscape();
	break;
      }
      case GhostAction.MOVING_ROOM: {
	debug_ui.SetDebug2("Ghost Action: Moving Room");
	state_MovingRoom();
	break;
      }
      case GhostAction.HIT_STUN: {
	debug_ui.SetDebug2("Ghost Action: Hit Stun");
	state_HitStun();
	break;
      }
      case GhostAction.POWER_WAVE: {
	debug_ui.SetDebug2("Ghost Action: Power - Wave");
	//state_HitStun();
	break;
      }
    }


  }

  void EnterAction(GhostAction action) {
    // Logic based on what state we're leaving
    switch (cur_action) {
      case GhostAction.CHARGING_ESCAPE: {
	charge_particles.Stop();
	break;
      }
    };

    // Enter New State Logic
    switch (action) {
      case GhostAction.CHARGING_ESCAPE: {
	// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
	charge_particles.Play();
	cur_action = action;
	break;
      }

      case GhostAction.MOVING_ROOM: {
	if (nav_destination == null) {
	  // Pick a room
	  // TODO: forbid this from being the room we're currently in
	  GameObject[] destinations = GameObject.FindGameObjectsWithTag("GhostDestination"); // Supposedly slow, but shouldn't be a big deal
	  int dest_idx = Random.Range(0, destinations.Length);
	  GameObject dest_obj = destinations[dest_idx];
	  nav_agent.destination = dest_obj.transform.position;
	  nav_destination = dest_obj;

	  // If we were moving when we got hit, we have not cleared our desired destination
	} else {
	  nav_agent.destination = nav_destination.transform.position;
	}

	cur_action = action;
	break;
      }

      case GhostAction.HIT_STUN: {
	ti_hit_stun_reset.reset();

	ti_hit_stun.reset();

	hit_stun_resistance += 0.08f;
	nav_agent.enabled = false;
	cur_action = action;
	break;
      }
    }

  }





  /** STATES **/

  void state_MovingRoom() {
    // Are we at the room? If so, start charging. Otherwise, keep moving.
    // Handcled in OnTriggerEnter
  }

  void state_ChargingEscape() {
    // TODO: be making wubwubwubwubwubwubwub sound

    float old_escape = escape_meter;
    escape_meter += Time.deltaTime;

    ui.UpdateEscapeMeter(escape_meter);

    //UIData.escape_meter = escape_meter;

    if (old_escape < escape_needed * 0.75 && escape_meter >= escape_needed * 0.75) {
      // TODO: spawn particle effect + sound
    } else if (old_escape < escape_needed*0.5 && escape_meter >= escape_needed * 0.5) {
      // TODO: spawn particle effect + sound
    } else if (old_escape < escape_needed*0.25 && escape_meter >= escape_needed * 0.25) {
      // TODO: spawn particle effect + sound
    }

    if (escape_meter >= escape_needed) {
      Debug.Log("You lose!");
    }
    // Can I see the player? Have I seen them for some amount of timer? Startle!
  }

  void state_HitStun() {
    ti_hit_stun.tick(Time.deltaTime);

    if (ti_hit_stun.finished_this_frame()) {
      nav_agent.enabled = true;

      EnterAction(GhostAction.MOVING_ROOM);

    }
  }

  // void state_Recovery() {
  //   // Idk... move away? Move to another room?
  // }





  /** EVENTS **/
  public void GetPunched(float power) {
    if (hit_stun_resistance > 0.3) {
      ti_hit_stun_reset.reset();
      return;
    }

    anim.SetTrigger("Punched");
    EnterAction(GhostAction.HIT_STUN); // Hmmm....

    // TODO: Track poise

  }


  public void OnTriggerEnter(Collider trigger) {
    if (trigger.gameObject == nav_destination) {
      nav_destination = null;
      EnterAction(GhostAction.CHARGING_ESCAPE);
    }
  }


  // TODO: wrap these in actual state changes so she doesn't keep trying to move around when she's ragdolled
  void EnableAnimator() {
    DisableRagdoll();
    anim.enabled = true;
  }

  void DisableAnimator() {
    anim.enabled = false;
  }

  void EnableRagdoll() {
    DisableAnimator();
    foreach (Collider col in rig_colliders) {
      col.enabled = true;
    }
    foreach (Rigidbody rb in rig_rbs) {
      rb.detectCollisions = true;
      rb.useGravity = true;
    }
    foreach (CharacterJoint joint in rig_joints) {
      joint.enableCollision = true;
    }
  }

  void DisableRagdoll() {
    foreach (Collider col in rig_colliders) {
      col.enabled = false;
    }
    foreach (Rigidbody rb in rig_rbs) {
      rb.detectCollisions = false;
      rb.useGravity = false;
    }
    foreach (CharacterJoint joint in rig_joints) {
      joint.enableCollision = false;
    }
  }
}
