
using UnityEngine;

public enum GetUpDirection {
	FACEDOWN,
	FACEUP,
}

public class GhostAction_GetUp : GhostAction {

	// For the ragdoll animator
	Timer ti_alpha_fade;
	float max_alpha;

	public GhostAction_GetUp(Ghost g, float init_max_alpha) : base(g) {
		// It would be nice if this eased...
		ti_alpha_fade = new Timer(1f, 1f);
		max_alpha = init_max_alpha;
	}

	public override void Enter() {
		Debug.Log("Entering get up");
		ti_alpha_fade.reset();

		// TODO: play an actual get up animation to fade into as well
	}

	public override void Exit() {
	}

	public override void Update() {
		ti_alpha_fade.tick(Time.deltaTime);
		
		ghost.ragdoll_animator.MasterAlpha = 0.2f * ti_alpha_fade.getPercentage();
	
		if (ti_alpha_fade.finished()) {
			ghost.ragdoll_animator.MasterAlpha = max_alpha;
			ghost.EnterAction(GhostActions.MOVING_ROOM);
		}
	}
	
}
