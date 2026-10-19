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

		ghost.flash_controller.StartFlashing(
			-1,
			ghost.defaults.stagger_large_data.FLASH_PERIOD,
			ghost.defaults.stagger_large_data.FLASH_MIN,
			ghost.defaults.stagger_large_data.FLASH_MAX
		);
	}

	public override void Exit() {
	}

	public override void Update() {

		// Update core timer
		ti_hit_stun.Tick(Time.deltaTime);

		if (ti_hit_stun.FinishedThisFrame()) {
			ghost.ChangeAnimation("StaggerLarge_End");
			ghost.flash_controller.Countdown(1);
			ti_recover.Activate();
		}

		ti_recover.Tick(Time.deltaTime);
		if (ti_recover.Finished()) {
			ghost.ExitAction();
		}
 	}

}
