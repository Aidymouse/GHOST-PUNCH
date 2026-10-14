
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
		ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;
		ghost.nav_agent.angularSpeed = ghost.defaults.rush_data.TURN_SPEED;
	}

	public override void Exit() {
		ghost.ChangeAnimation("Idle");
		ghost.nav_agent.angularSpeed = orig_turn_speed;
	}

	public override void Update() {
		ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;
		ghost.LookAt(ghost.nav_agent.steeringTarget);

		if (ghost.nav_agent.remainingDistance <= ghost.defaults.rush_data.CLOSE_ENOUGH) {
			// TODO: small chance we keep running?
			ghost.ExitAction();
		}
	}
	
}
