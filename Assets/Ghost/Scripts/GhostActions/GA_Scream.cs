using UnityEngine;

public class GA_Scream : GhostAction {

	Timer ti_scream_charge;
	Timer ti_scream;

	public GA_Scream(Ghost g) : base(g) {
		float scream_anim_duration = AnimUtil.GetClipLength("Scream", g.animator);
		ti_scream = new Timer(0, scream_anim_duration + ghost.defaults.scream_data.HANG_TIME);
		ti_scream_charge = new Timer(0, ghost.defaults.scream_data.DELAY);
		Debug.Log("Delay" + ghost.defaults.scream_data.DELAY);
	}

	public override void Enter() {
		ti_scream_charge.Reset();
		ti_scream.Reset();
		ghost.ChangeAnimation("Scream");
	}

	public override void Update() {
		ti_scream_charge.Tick(Time.deltaTime);
		if (ti_scream_charge.FinishedThisFrame()) {

			float dist_to_puncher = GhostUtils.TopDownDistance(ghost.transform.position, ghost.ghostPuncher.transform.position);

			Debug.Log("Launching scream with range " + ghost.defaults.scream_data.RANGE + ", puncher is " + dist_to_puncher + "m away");

			if (dist_to_puncher <= ghost.defaults.scream_data.RANGE) {
				S_Slowed slowed = new S_Slowed(ghost.defaults.scream_data.SPEED_REDUCTION_DURATION, ghost.defaults.scream_data.SPEED_REDUCTION);
				ghost.ghostPuncher.AddStatus(slowed);
			}

		}

		ti_scream.Tick(Time.deltaTime);
		if (ti_scream.Finished()) {
			ghost.ExitAction();
		}
	}
	
	public override string GetDebugString() {
		string scream_debug = "";
		
		scream_debug += "Scream Charge: " + ti_scream_charge.GetTime() + " (" + ti_scream_charge.GetPercentage() + ")" + "\n";
		scream_debug += "Scream timer: " + ti_scream.GetTime() + " (" + ti_scream.GetPercentage() + ")";

		return scream_debug;
		
	}

	
}
