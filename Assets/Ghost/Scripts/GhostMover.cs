using UnityEngine;
using UnityEngine.AI;


/** Struct with some basic nav agent stuff so we can pop and push to the stack based on what we're doing, so we can keep track of how we're moving and revert states gracefully */
public struct NavInfo {
	public float speed;
	public float angularSpeed;
	public float acceleration;
	public float stoppingDistance;

	public NavInfo(float s, float angs, float acc, float sd) {
		speed = s;
		angularSpeed = angs;
		acceleration = acc;
		stoppingDistance = sd;
	}

	public NavInfo(NavInfo prev_info) {
		speed = prev_info.speed;
		angularSpeed = prev_info.angularSpeed;
		acceleration = prev_info.acceleration;
		stoppingDistance = prev_info.stoppingDistance;
	}
}

/** Encapsulates both a destination and custom movement variables */
public struct NavState {
	/** If present, move like this. Otherwise just use the default */
	public NavInfo? info;
	/** Destination to go to */
	public Vector3 destination;
	/** If true, ghost will try to look at her current steering target */
	public bool look_where_im_going;
	/** Why'd we push this nav state, for debug */
	public string reason;

	public NavState(Vector3 dest, NavInfo? inf, bool look, string reas = "") {
		destination = dest;
		info = inf;
		look_where_im_going = look;
		reason = reas;
	}
}


/** Manages ghost movement state + has a nice API for telling ghost to go somewhere as well as HOW to get there */
public class GhostMover : MonoBehaviour {

	public Ghost ghost;
  public NavMeshAgent nav_agent;
	/** Whenever my pos is set by wandering, set this. If the nav agents position differs from this, im not wandering anymore */

	[Header("Circling")]
	[Tooltip("When the ghost is circling, each point is at most this far")]
	public float max_circling_range;

	public NavState nav_state;

	public Vector3 circling_pos;

	/** If the stack is empty, revert to this base info, harvested from default nav agent values on Awake */
	public NavInfo base_nav_info;
	/** Nav info we're currently using to move */
	NavState? cur_state;

	public void Awake() {

		base_nav_info = new NavInfo(
			nav_agent.speed,
			nav_agent.angularSpeed,
			nav_agent.acceleration,
			nav_agent.stoppingDistance
		);

		Debug.Log("nav info stopping distance: " + base_nav_info.stoppingDistance);
	
	}

	public void Update() {
		if (nav_agent.isStopped && cur_state is not null) {
			ApplyNavInfo(base_nav_info);
			cur_state = null;
			return;
		}

		if (nav_agent.remainingDistance <= nav_agent.stoppingDistance) {
			/** No matter what the ghosts action, she's now circling
 			*	TODO: this will need to be based on more stuff than just stopping distance */

			ApplyNavInfo(base_nav_info);
			cur_state = null;


			// TODO: if within medium range
			StartCircling();
		}

		if (cur_state?.look_where_im_going ?? false) {
			ghost.LookAt(nav_agent.steeringTarget);
		}

	}

	// This should be used instead of setting nav agent directly
	public void GoSomewhere(NavState new_state) {
		cur_state = new_state;

		nav_agent.destination = new_state.destination;

		if (new_state.info is not null) {
			ApplyNavInfo(new_state.info.Value);
		}
		
	}

	/** For updating only position but retaining cur info */
	public void UpdateDestination(Vector3 new_pos) {
		nav_agent.destination = new_pos;
	}


	public void StartCircling() {
		Vector3? rand_p = NavMeshUtil.GetRandomPoint(transform.position, max_circling_range);
		if (rand_p is not null) {

			NavInfo circle_info = new NavInfo(1, 180, 0.5f, 0);
				
			NavState circling_state = new NavState(rand_p.Value, circle_info, false, "Circling");
			GoSomewhere(circling_state);
		}
	}

	public void ApplyNavInfo(NavInfo info) {
		nav_agent.speed = info.speed;
		nav_agent.angularSpeed = info.angularSpeed;
		nav_agent.acceleration = info.acceleration;
		nav_agent.stoppingDistance = info.stoppingDistance;
	}

	/** 
 	* Nav Agent Stack Methods 
 	* */

	/** If the ghost doesn't end her movement state by reaching the destination (e.g. ragdoll), we probably want to clear the stack */
	public void ClearStack() {
		// TODO:
	}

	/** Control methods */
	public void Stop() {
		nav_agent.isStopped = true;
	}

	public string GetDebugString() {
		string s = "";

		s += "Nav Agent {\n";
		s += "  isStopped: " + nav_agent.isStopped + "\n";
		s += "  speed: " + nav_agent.speed + "\n";
		s += "  angularSpeed: " + nav_agent.angularSpeed + "\n";
		s += "  acceleration: " + nav_agent.acceleration + "\n";
		s += "  stoppingDistance: " + nav_agent.stoppingDistance + "\n";
		s += "}\n";
		s += "Going to " + nav_agent.destination + "\n";
		s += "  because " + (cur_state?.reason ?? "-") + "\n";
		s += "Remaining distance: " + nav_agent.remainingDistance + "\n";
		if (ghost.look_target is not null) {
			s += "Looking at: " + ghost.look_target;
		} else if (ghost.look_at is not null) {
			s += "Looking at: " + ghost.look_at.Value;
		} else {
			s += "Looking at: -";
		}

		s += "\n";


		return s;
	}

	/** Path following methods */
	//public void FollowPath() { }

}
