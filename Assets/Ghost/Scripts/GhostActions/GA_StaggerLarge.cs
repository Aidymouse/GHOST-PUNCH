using UnityEngine;

public class GA_StaggerLarge : GhostAction {

	public GA_StaggerLarge(Ghost g) : base(g) {}

	public override void Enter() {
				ghost.ti_hit_stun.Reset();
				ghost.PlayAnimation("Hit_Cower");
				ghost.nav_agent.isStopped = true;
	}

	public override void Update() {
		ghost.ti_hit_stun.Tick(Time.deltaTime);

		if (ghost.ti_hit_stun.FinishedThisFrame()) {
			ghost.ti_recovery.Set(0);
			ghost.EnterAction(GhostActions.RECOVERY);
		}
 	}

}
