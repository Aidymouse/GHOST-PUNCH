
using UnityEngine;

/** The ghost rushes towards the puncher! */
public class GA_Rush : GhostAction {

	float orig_turn_speed;

	public GA_Rush(Ghost g) : base(g) {
		orig_turn_speed = g.nav_agent.angularSpeed;
	}


	public override void Enter() {
		ghost.ChangeAnimation("Rush");
		// For some reason if I don't set this right here right now the action ends instantly / over and over and the idle animation takes over
		// Even though the ghost gets where she needs to???

		//ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;
		//ghost.nav_agent.angularSpeed = ghost.defaults.rush_data.TURN_SPEED;

		NavInfo rush_info = new NavInfo(ghost.mover.base_nav_info);
		rush_info.angularSpeed = ghost.defaults.rush_data.TURN_SPEED;

		NavState rush_nav_state = new NavState(ghost.ghostPuncher.transform.position, rush_info, true, "Rush Action");

		ghost.mover.GoSomewhere(rush_nav_state);
		

	}

	public override void Exit() {
		ghost.ChangeAnimation("Idle");
		// Revert now happens in GhostMover
		//ghost.nav_agent.angularSpeed = orig_turn_speed;
	}

	public override void Update() {
		ghost.mover.UpdateDestination(ghost.ghostPuncher.transform.position);
		// GhostMover does this now
		//ghost.LookAt(ghost.nav_agent.steeringTarget);

		if (ghost.nav_agent.remainingDistance <= ghost.defaults.rush_data.CLOSE_ENOUGH) {
			// TODO: small chance we keep running?
			ghost.ExitAction();
		}
	}
	
}
