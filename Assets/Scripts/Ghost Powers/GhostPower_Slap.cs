using UnityEngine;
class GhostPower_Slap : GhostPower {
  public GhostPower_Slap(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.SLAP_CHARGE_TIME, attrs.SLAP_ACTIVE_DELAY_TIME, attrs.SLAP_ACTIVE_TIME, attrs.SLAP_HANG_TIME) {
  }

  public override void Start() {
	Vector3 ghostPuncher_position = ghost.ghostPuncher.transform.position;
	this.ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;
	this.ghost.nav_agent.stoppingDistance = attrs.SLAP_DISTANCE;
	// Intentionally don't start timer phases yet
  }

  public override void Update() {
    // Move up to ghost puncher
    if (phase == GhostPowerPhase.PRE_CHARGE) {
      ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;

      if (ghost.nav_agent.remainingDistance <= attrs.SLAP_DISTANCE) {
	ghost.nav_agent.isStopped = true;
	ti_charge.activate();
	OnStartCharge();
      }
    }

    UpdateTimers();
    HandleEvents();
  }

  public override void End() {
	// TODO: make this some point in the room
	ghost.nav_agent.destination = ghost.transform.position;
	ghost.nav_agent.stoppingDistance = 0;
	ghost.nav_agent.isStopped = false;
  }

  public override void OnEndActiveDelay() {
      // TODO: turn the collider hot
      ghost.ChangeAnimation("Power_SlapLeft");
  }

  public override void OnEndActive() {
    // TODO: turn collider cold
  }

}

