using UnityEngine;

public class GA_StaggerLarge : GhostAction {

	Timer ti_hit_stun;
	// For while the ghost exists the stagger
	Timer ti_recover;

	public GA_StaggerLarge(Ghost g) : base(g) {
    ti_hit_stun = new Timer(0, g.defaults.stagger_large_data.STAGGER_TIME);

		ti_recover = new Timer(0, AnimUtil.GetClipLength("StaggerLargeEnd", ghost.animator));
	}

	public override void Enter() {
		ti_hit_stun.Reset();
		ti_recover.Reset();
		ti_recover.Deactivate();
		ghost.PlayAnimation("StaggerLarge");
		ghost.nav_agent.isStopped = true;
		//ghost.nav_agent.destination = null;
	}

	public override void Exit() {
	}

	public override void Update() {
		ti_hit_stun.Tick(Time.deltaTime);

		if (ti_hit_stun.FinishedThisFrame()) {
			ghost.ChangeAnimation("StaggerLarge_End");
			ti_recover.Activate();
		}

		ti_recover.Tick(Time.deltaTime);
		if (ti_recover.Finished()) {
			ghost.ExitAction();
		}
 	}

}
