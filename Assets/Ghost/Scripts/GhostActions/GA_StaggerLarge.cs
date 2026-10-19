using UnityEngine;

public class GA_StaggerLarge : GhostAction {

	Timer ti_hit_stun;
	// For while the ghost exists the stagger
	Timer ti_recover;
	Timer ti_flash;
	int flash_dir;

	public GA_StaggerLarge(Ghost g) : base(g) {
    ti_hit_stun = new Timer(0, g.defaults.stagger_large_data.STAGGER_TIME);

		ti_flash = new Timer(0, g.defaults.stagger_large_data.FLASH_SPEED);
		ti_recover = new Timer(0, AnimUtil.GetClipLength("StaggerLargeEnd", ghost.animator));
		flash_dir = 1;
	}

	public override void Enter() {
		ti_hit_stun.Reset();
		ti_recover.Reset();
		ti_recover.Deactivate();
		ghost.PlayAnimation("StaggerLarge");
		ghost.nav_agent.isStopped = true;

		ti_flash.Activate();
		ti_flash.Reset();
		flash_dir = 1;
		//ghost.nav_agent.destination = null;
	}

	public override void Exit() {
	}

	public override void Update() {

		// Update Flash
		ti_flash.Tick(Time.deltaTime);
		float flash_prop = ti_flash.GetPercentage();
		if (flash_dir == -1) {
			flash_prop = 1 - ti_flash.GetPercentage();
		}

		ghost.mesh_renderer.material.SetFloat("_FlashAmount", Lerp.Linear(ghost.defaults.stagger_large_data.FLASH_MIN, ghost.defaults.stagger_large_data.FLASH_MAX, flash_prop));

		if (ti_flash.FinishedThisFrame()) {
			ti_flash.Reset();
			flash_dir = -flash_dir;
			if (ti_recover.IsActive() && ghost.mesh_renderer.material.GetFloat("_FlashAmount") == 0) {
				ti_flash.Deactivate();
			}
		}

		// Update core timer
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
