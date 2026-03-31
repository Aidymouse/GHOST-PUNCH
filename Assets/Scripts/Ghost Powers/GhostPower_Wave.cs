using UnityEngine;

class GhostPower_Wave : GhostPower {
  public GhostPower_Wave(Ghost myghost, GhostPowerAttributes attrs) : base(myghost, attrs.WAVE_CHARGE_TIME, 0, 0, attrs.WAVE_HANG_TIME) {
  }

  public void OnEndCharge() {

      // Play animation
      // TODO:

      // Spawn wave orb
      Instantiate(wave_orb, transform.position, new Quaternion()); 
  }
}
