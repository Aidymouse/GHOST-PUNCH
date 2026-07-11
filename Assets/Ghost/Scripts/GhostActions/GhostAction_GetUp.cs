
using UnityEngine;

/*
public enum GetUpDirection {
	FACEDOWN,
	FACEUP,
}
*/

public class GhostAction_GetUp : GhostAction {

	// For the ragdoll animator
	Timer ti_alpha_fade;
	float max_alpha;

	public GhostAction_GetUp(Ghost g, float init_max_alpha) : base(g) {
		// It would be nice if this eased...
		ti_alpha_fade = new Timer(1.0f, 1.0f);
		max_alpha = init_max_alpha;
	}

	public override void Enter() {
		Debug.Log("Entering get up");

		//ghost.ragdoll_animator.MasterAlpha = max_alpha;
		//ghost.EnterAction(GhostActions.MOVING_ROOM);

		Vector3 ragdoll_offset = ghost.transform.position - ghost.rig_core.transform.position;
		ragdoll_offset.y = 0;
		ghost.transform.position -= ragdoll_offset;
		ghost.rig_core.transform.position += ragdoll_offset;
		ghost.ragdoll_settings.PowerProfile = ghost.ragprof_animated;


		// TODO: could track how she facing 
		ghost.ChangeAnimation("GetUpFaceup");

	}

	public override void Exit() {
		Debug.Log("Exit get up");
	}

	public override void Update() {

		//ghost.rig_core.AddForce(-ghost.rig_core.linearVelocity);


		ti_alpha_fade.tick(Time.deltaTime);
		
		ghost.ragdoll_animator.MasterAlpha = 0.2f * ti_alpha_fade.getPercentage();
	
		if (ti_alpha_fade.finished()) {
			Debug.Log("Get Up should end now");
			ghost.ragdoll_animator.MasterAlpha = max_alpha;
			ghost.EnterAction(GhostActions.MOVING_ROOM);
		}
	}
	
}
