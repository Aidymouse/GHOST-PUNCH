using UnityEngine;

class GhostPower_Slap : GhostPower {
  public GhostPower_Slap(Ghost myghost) : base(myghost) {
  }

  public void Start() {
	Vector3 ghostPuncher_position = this.ghost.ghostPuncher.transform.position;
	this.ghost.nav_agent.destination = ghostPuncher.transform.position;
	this.ghost.nav_agent.stoppingDistance = power_attribs.SLAP_DISTANCE;
	// Intentionally don't start timer phases yet
  }

  public void Update() {
    // Move up to ghost puncher
    if (phase === GhostPowerPhase.PRE_CHARGE) {
      nav_agent.destination = ghostPuncher.transform.position;

      if (nav_agent.remainingDistance <= power_attribs.SLAP_DISTANCE /*&& !ti_power_charge.is_active()*/ && !ti_power_hang.is_active()) {
	ti_power_charge.time_remaining = power_attribs.SLAP_CHARGE_TIME;
	ti_power_charge.activate();
	OnStartCharge();
      }
    }

    UpdateTimers();
    HandleEvents();
  }

  public void End() {
	// TODO: make this a point in the room
	nav_agent.destination = transform.position;
	nav_agent.stoppingDistance = 0;
  }

  public void OnActiveDelayEnd() {
      // TODO: turn the collider hot
      ghost.ChangeAnimation("Power_SlapLeft");
  }

  public void OnActiveEnd() {
    // TODO: turn collider cold
  }
}

