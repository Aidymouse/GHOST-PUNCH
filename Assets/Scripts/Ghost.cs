using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;

using UnityEditor;

public class Ghost : MonoBehaviour
{

    //public GhostUI UIData;
    //
    public GhostUI ui;
    public DebugUI debug_ui;

    public float escape_meter;
    public float escape_needed;

    GameObject nav_destination;
    ParticleSystem charge_particles;
    Animator anim;

    enum GhostAction {
      CHARGING_ESCAPE,
      STARTLED,
      USING_POWER,
      HITSTUN,
      MOVING_ROOM,
      RECOVERY,
      HIT_STUN,
      RAGDOLL,
    }

    GhostAction cur_action;

    NavMeshAgent nav_agent;

    float timer_hit_stun;
    // Reset every time the ghost gets hit
    float timer_hit_stun_reset;
    // Reduces hit stun and increases with each hit until reset
    float hit_stun_resistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


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

      // TODO: make this only affect the skeleton rig
      //transform.position += new Vector3(0, Mathf.Sin(escape_meter), 0);

      debug_ui.SetDebug1("Ghost Hitstun timer: " + timer_hit_stun);

      if (timer_hit_stun_reset > 0) {
	timer_hit_stun_reset -= Time.deltaTime;
	if (timer_hit_stun_reset <= 0) {
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
	// case GhostAction.RECOVERY: {
	//   // TODO
	//   state_Recovery();
	//   break;
	// }
	case GhostAction.HIT_STUN: {
	  debug_ui.SetDebug2("Ghost Action: Hit Stun");
	  state_HitStun();
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
      }
      
      // Enter New State Logic
      if (action == GhostAction.CHARGING_ESCAPE) {
	// TODO: If we can see the player, skip straight to choosing a power.
	charge_particles.Play();
	cur_action = action;

      } else if (action == GhostAction.MOVING_ROOM) {

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

	debug_ui.SetDebug3(nav_destination.name);

	cur_action = action;

      } else if (action == GhostAction.HIT_STUN) {
	timer_hit_stun_reset = 1;

	timer_hit_stun = 0.3f; // - hit_stun_resistance;

	hit_stun_resistance += 0.08f;
	nav_agent.enabled = false;
	cur_action = action;

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
      timer_hit_stun -= Time.deltaTime;

      if (timer_hit_stun <= 0) {
	nav_agent.enabled = true;

	EnterAction(GhostAction.MOVING_ROOM);

      }
    }

    // void state_Recovery() {
    //   // Idk... move away? Move to another room?
    // }





    /** EVENTS **/
    public void GetPunched() {
      if (hit_stun_resistance > 0.3) {
	timer_hit_stun_reset = 1;
	return;
      }

      anim.SetTrigger("Punched");
      EnterAction(GhostAction.HIT_STUN);
    }


    public void OnTriggerEnter(Collider trigger) {
      if (trigger.gameObject == nav_destination) {
	nav_destination = null;
	debug_ui.SetDebug3("");
	EnterAction(GhostAction.CHARGING_ESCAPE);
      }
    }

}
