using UnityEngine;

public class GhostAction_Recovery : GhostAction {


	public GhostAction_Recovery(Ghost g) : base(g) {}

	public override void Enter() { 
		ghost.ti_recovery.activate();
		// TODO: if the ghost was attacking this should probably be 0...
		ghost.ChangeAnimation("Idle", ghost.ti_recovery.time_remaining);
	}

	public override void Update() {
		ghost.ti_recovery.tick(Time.deltaTime);

		if (ghost.ti_recovery.finished()) {
			ghost.RestorePoise();
			ghost.nav_agent.isStopped = false;

			ghost.EnterAction(GhostActions.USING_POWER);
			/*
				 if (nav_destination == null) {
				 } else {
				 EnterAction(GhostActions.MOVING_ROOM);
				 }
				 */
		}
	}

}
