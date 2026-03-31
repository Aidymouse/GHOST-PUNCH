using UnityEngine;
/*
class GhostPower_Slap : GhostPower {
  public GhostPower_Slap(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.SLAP_CHARGE_TIME, 0, 0, 0) {
  }

  new public void Start() {
	Vector3 ghostPuncher_position = this.ghost.ghostPuncher.transform.position;
	this.ghost.nav_agent.destination = ghostPuncher.transform.position;
	this.ghost.nav_agent.stoppingDistance = attrs.SLAP_DISTANCE;
	// Intentionally don't start timer phases yet
  }

  public void Update() {
    // Move up to ghost puncher
    if (phase == GhostPowerPhase.PRE_CHARGE) {
      ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;

      if (ghost.nav_agent.remainingDistance <= power_attribs.SLAP_DISTANCE /*&& !ti_power_charge.is_active()*/ /*&& !ti_power_hang.is_active()) {
	ti_charge.activate();
	OnStartCharge();
      }
    }

    UpdateTimers();
    HandleEvents();
  }

  public void End() {
	// TODO: make this a point in the room
	nav_agent.destination = ghost.transform.position;
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

*/
