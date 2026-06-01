using UnityEngine;

public class GhostAction_HitStun : GhostAction {

	public GhostAction_HitStun(Ghost g) : base(g) {}

	public override void Enter() {
				ghost.ti_hit_stun.reset();
				ghost.PlayAnimation("Hit_Cower");
				ghost.nav_agent.isStopped = true;
	}

	public override void Update() {
		ghost.ti_hit_stun.tick(Time.deltaTime);

		if (ghost.ti_hit_stun.finished_this_frame()) {
			ghost.ti_recovery.set(0);
			ghost.EnterAction(GhostActions.RECOVERY);
		}
 	}

}
