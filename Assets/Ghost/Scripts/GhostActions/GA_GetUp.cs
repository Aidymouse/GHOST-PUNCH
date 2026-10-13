
using UnityEngine;

/*
public enum GetUpDirection {
	FACEDOWN,
	FACEUP,
}
*/

public class GA_GetUp : GhostAction {

	// For the ragdoll animator
	Timer ti_alpha_fade;
	float max_alpha;

	public GA_GetUp(Ghost g, float init_max_alpha) : base(g) {
		// It would be nice if this eased...
		ti_alpha_fade = new Timer(1.0f, 1.0f);
		//max_alpha = init_max_alpha;
		max_alpha = 1;
	}

	public override void Enter() {
		Debug.Log("Entering get up");

		//ghost.ragdoll_animator.MasterAlpha = max_alpha;
		//ghost.EnterAction(GhostActions.MOVING_ROOM);

		// Since the ragdoll is part of the ghost parent, it actually flies off and the transform is all messed up. This code syncs the positions again
		Vector3 ragdoll_offset = ghost.transform.position - ghost.rig_core.transform.position;
		ragdoll_offset.y = 0;
		ghost.transform.position -= ragdoll_offset;
		ghost.rig_core.transform.position += ragdoll_offset;
		ghost.ragdoll_settings.PowerProfile = ghost.ragprof_animated;

		ti_alpha_fade.Reset();


		// TODO: could track how she facing 
		//ghost.ChangeAnimation("GetUpFaceup");
		ghost.ChangeAnimation("Crawl");

		ghost.StopBeingVulnerable();

	}

	public override void Exit() {
		Debug.Log("Exit get up");
		ghost.ragdoll_animator.MasterAlpha = max_alpha;
		ghost.ragdoll_animator.forceTargetPose = true;
	}

	public override void Update() {

		//ghost.rig_core.AddForce(-ghost.rig_core.linearVelocity);


		ti_alpha_fade.Tick(Time.deltaTime);
		
		ghost.ragdoll_animator.MasterAlpha = 0.2f * ti_alpha_fade.GetPercentage();
	
		if (ti_alpha_fade.Finished()) {
			ghost.ExitAction(GhostActions.SCURRY);
		}
	}
	
}
