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
		if ((ghost.ghostPuncher.transform.position - ghost.transform.position).magnitude < attrs.SCREAM_HIT_DISTANCE) {


			StatusEffect slowed = new S_Slowed(attrs.SCREAM_SPEED_REDUCTION_DURATION, attrs.SCREAM_SPEED_REDUCTION);

			ghost.ghostPuncher.GetComponent<GhostPuncher>().AddStatus(slowed);
		}
	}

}
