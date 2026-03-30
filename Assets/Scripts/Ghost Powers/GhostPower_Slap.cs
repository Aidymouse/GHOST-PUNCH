using UnityEngine;

class GhostPower_Slap : GhostPower {
  public GhostPower_Slap(Ghost myghost) : base(myghost) {
  }

  public void Start() {
	Vector3 ghostPuncher_position = this.ghost.ghostPuncher.transform.position;
	this.ghost.nav_agent.destination = ghostPuncher.transform.position;
	this.ghost.nav_agent.stoppingDistance = power_attribs.SLAP_DISTANCE;


      

  }

  public void Update() {
    // Move up to ghost puncher
  }

  public void End() {
	// TODO: make this a point in the room
	nav_agent.destination = transform.position;
	nav_agent.stoppingDistance = 0;
  }
}

