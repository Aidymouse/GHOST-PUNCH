using UnityEngine;

class GhostPower_Scream : GhostPower {
	public GhostPower_Scream(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.SCREAM_CHARGE_TIME, attrs.SCREAM_ACTIVE_DELAY_TIME, attrs.SCREAM_ACTIVE_TIME, attrs.SCREAM_HANG_TIME) {
	}

	public override void OnEndCharge() {
		ghost.ChangeAnimation("Scream");
	}
	public override void OnEndActiveDelay() {
		// TODO: spawn cylinder? 
		// Anything within is 
	}

}
