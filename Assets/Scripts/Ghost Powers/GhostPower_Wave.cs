using UnityEngine;

class GhostPower_Wave : GhostPower {
  public GhostPower_Wave(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.WAVE_CHARGE_TIME, 0, 0, attrs.WAVE_HANG_TIME) {
  }

  public override void OnEndCharge() {

      // Play animation
      // TODO:

      // Spawn wave orb
      Ghost.Instantiate(ghost.wave_orb, ghost.transform.position, new Quaternion()); 
  }
}
