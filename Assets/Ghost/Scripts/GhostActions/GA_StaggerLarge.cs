using UnityEngine;

public class GA_StaggerLarge : GhostAction {

	Timer ti_hit_stun;

	public GA_StaggerLarge(Ghost g) : base(g) {
    ti_hit_stun = new Timer(0, g.defaults.stagger_large_data.STAGGER_TIME);
	}

	public override void Enter() {
		ti_hit_stun.Reset();
		ghost.PlayAnimation("StaggerLarge");
	}

	public override void Exit() {
		ghost.PlayAnimation("StaggerLarge_End");
	}

	public override void Update() {
		ti_hit_stun.Tick(Time.deltaTime);

		if (ti_hit_stun.FinishedThisFrame()) {
			ghost.ExitAction();
		}
 	}

}
