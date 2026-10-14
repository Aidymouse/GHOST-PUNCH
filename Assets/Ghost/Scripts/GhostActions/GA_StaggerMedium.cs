using UnityEngine;

public class GA_StaggerMedium : GhostAction {

	Timer ti_hit_stun;

	public GA_StaggerMedium(Ghost g) : base(g) {
    ti_hit_stun = new Timer(0, g.defaults.stagger_medium_data.STAGGER_TIME);
	}

	public override void Enter() {
		ti_hit_stun.Reset();
		ghost.PlayAnimation(ghost.defaults.stagger_medium_data.ANIMATION);
		//ghost.nav_agent.isStopped = true;
	}

	public override void Update() {
		ti_hit_stun.Tick(Time.deltaTime);

		if (ti_hit_stun.FinishedThisFrame()) {
			ghost.ExitAction();
		}
 	}

}
