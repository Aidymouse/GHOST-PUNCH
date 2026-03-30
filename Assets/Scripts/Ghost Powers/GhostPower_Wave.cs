using UnityEngine;

class GhostPower_Wave : GhostPower {
  public GhostPower_Wave(Ghost myghost) : base(myghost) {
  }

  public void Start() {
	this.ti_charge.set(power_attribs.WAVE_CHARGE_TIME);
	this.ti_charge.activate();
	this.ti_hang.set(power_attribs.WAVE_HANG_TIME);
	this.ti_hang.deactivate();

	this.ghost.cur_power = GhostPowers.WAVE;
      }

  }

  public void EndCharge() {

      // Play animation
      // TODO:

      // Spawn wave orb
      Instantiate(wave_orb, transform.position, new Quaternion()); 
  }
}
