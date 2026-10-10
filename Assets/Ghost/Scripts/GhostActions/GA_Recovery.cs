using UnityEngine;

public class GA_Recovery : GhostAction {

  Timer ti_recovery = new Timer(0);

	public GA_Recovery(Ghost g) : base(g) {}

	public override void Enter() { 
		// TODO: if the ghost was attacking this should probably be 0...
		ghost.ChangeAnimation("Idle", ti_recovery.time_remaining);
		ti_recovery.Set(1);
		ti_recovery.Activate();
	}

	public override void Update() {
		ti_recovery.Tick(Time.deltaTime);

		if (ti_recovery.Finished()) {
			ghost.nav_agent.isStopped = false;

			ghost.ExitAction();
			//ghost.EnterAction(GhostActions.USING_POWER);
			/*
				 if (nav_destination == null) {
				 } else {
				 EnterAction(GhostActions.MOVING_ROOM);
				 }
				 */
		}
	}

}
