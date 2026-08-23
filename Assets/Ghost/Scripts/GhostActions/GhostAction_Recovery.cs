using UnityEngine;

public class GhostAction_Recovery : GhostAction {


	public GhostAction_Recovery(Ghost g) : base(g) {}

	public override void Enter() { 
		ghost.ti_recovery.Activate();
		// TODO: if the ghost was attacking this should probably be 0...
		ghost.ChangeAnimation("Idle", ghost.ti_recovery.time_remaining);
	}

	public override void Update() {
		ghost.ti_recovery.Tick(Time.deltaTime);

		if (ghost.ti_recovery.Finished()) {
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
