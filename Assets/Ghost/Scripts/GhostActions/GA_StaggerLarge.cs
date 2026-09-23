using UnityEngine;

public class GA_StaggerLarge : GhostAction {

	Timer ti_hit_stun;

	public GA_StaggerLarge(Ghost g) : base(g) {
    ti_hit_stun = new Timer(0, g.defaults.HIT_STUN_TIME);
	}

	public override void Enter() {
				ti_hit_stun.Reset();
				ghost.PlayAnimation("Hit_Cower");
				ghost.nav_agent.isStopped = true;
	}

	public override void Update() {
		ti_hit_stun.Tick(Time.deltaTime);

		if (ti_hit_stun.FinishedThisFrame()) {
			ghost.ti_recovery.Set(0);
			ghost.EnterAction(GhostActions.RECOVERY);
		}
 	}

}
