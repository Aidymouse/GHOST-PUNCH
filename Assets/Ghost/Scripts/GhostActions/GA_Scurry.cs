
using UnityEngine;

// TODO:
public class GA_Scurry : GhostAction {

	float base_angular_speed;
	float base_speed;
	float scurry_stopping_distance;

	public GA_Scurry(Ghost g) : base(g) {
		base_angular_speed = g.nav_agent.angularSpeed;
		base_speed = g.nav_agent.speed;
		scurry_stopping_distance = g.mover.base_nav_info.stoppingDistance;
	}

	public override void Enter() {
		// Find a point somewhere
		Vector3? rand_pos = NavMeshUtil.GetRandomPoint(ghost.transform.position, 10, 5, true);

		if (rand_pos is not null) {
			ghost.ChangeAnimation(ghost.defaults.scurry_data.animation_name);
			ghost.ragdoll_animator.forceTargetPose = true;
			ghost.ragdoll_animator.MasterAlpha = 1;

			ghost.nav_agent.isStopped = false;

			NavInfo scurry_nav_info = new NavInfo(ghost.mover.base_nav_info);
			scurry_nav_info.speed = ghost.defaults.scurry_data.speed;
			scurry_nav_info.angularSpeed = ghost.defaults.scurry_data.turn_speed;

			ghost.mover.GoSomewhere(new NavState(rand_pos.Value, scurry_nav_info, true, "Scurry"));

		} else {

			ghost.EnterAction(GhostActions.GET_UP);

		}


		// Change to scurry anim
		// Get there
		// Maybe just walk random ??
	}


	public override void Exit() {
			ghost.ChangeAnimation("Idle");
			//ghost.nav_agent.angularSpeed = base_angular_speed;
			//ghost.nav_agent.speed = base_speed;
	}

	public override void Update() {

		//Debug.Log(ghost.nav_agent.destination + ", " + ghost.nav_agent.remainingDistance);

		//ghost.LookAt(ghost.nav_agent.steeringTarget);

		if (ghost.nav_agent.remainingDistance <= scurry_stopping_distance) {
			ghost.ExitAction();
		}
	}
	
}

/*
  void CheckJumpscareTrigger()
  {
    if (!jumpscareReady) return;
    float dist = Vector3.Distance(transform.position, ghostPuncher.transform.position);

		if (dist <= jumpscareDistance)
		{
			TriggerJumpscare();
		}
	}

  public void TriggerJumpscare()
  {
    if (inJumpscare) return;
    inJumpscare = true;
    Debug.Log("JUMPSCARED");

    // Freeze AI
    nav_agent.isStopped = true;

    // align point to target, this doesn't fucking work.
    Vector3 offset = transform.position - jumpscareAlignPoint.position;
    transform.position = jumpscareTarget.position + offset;
    transform.rotation = jumpscareTarget.rotation;

    // Lock player movement
    ghostPuncher.inCutscene = true;

    // Play timeline
    jumpscareTimeline.time = 0;
    jumpscareTimeline.Play();
  }

  public void EndJumpscare()
  {
    inJumpscare = false;
    nav_agent.enabled = true;
    nav_agent.updatePosition = true;
    nav_agent.updateRotation = true;
    anim.enabled = true;
    ghostPuncher.GetComponent<GhostPuncher>().inCutscene = false;
  }
*/
