using UnityEngine;

class GhostPower_Wave : GhostPower {
  public GhostPower_Wave(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.WAVE_CHARGE_TIME, attrs.WAVE_ACTIVE_DELAY_TIME, attrs.WAVE_ACTIVE_TIME, attrs.WAVE_HANG_TIME) {
  }

	public override void OnEndCharge() {
      ghost.ChangeAnimation("Blast");
	}

  public override void OnEndActiveDelay() {
      // Spawn wave orb
      Ghost.Instantiate(ghost.wave_orb, ghost.transform.position, new Quaternion()); 
  }
}
