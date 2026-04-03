using UnityEngine;
using UnityEngine.AI;

class GhostPower_Slap : GhostPower {

	NavMeshAgent nav_agent;

	public GhostPower_Slap(Ghost myghost, GhostPowerAttribs attrs) : base(myghost, attrs, attrs.SLAP_CHARGE_TIME, attrs.SLAP_ACTIVE_DELAY_TIME, attrs.SLAP_ACTIVE_TIME, attrs.SLAP_HANG_TIME) {
		nav_agent = this.ghost.get_nav_agent();
	}

	public override void Start() {
		NavMeshAgent nav_agent = this.ghost.get_nav_agent();

		Vector3 ghostPuncher_position = ghost.ghostPuncher.transform.position;
		nav_agent.destination = ghost.ghostPuncher.transform.position;
		nav_agent.stoppingDistance = attrs.SLAP_DISTANCE;
		// Intentionally don't start timer phases yet
	}

	public override void Update() {
		// Move up to ghost puncher
		if (phase == GhostPowerPhase.PRE_CHARGE) {
			nav_agent.destination = ghost.ghostPuncher.transform.position;

			if (nav_agent.remainingDistance <= attrs.SLAP_DISTANCE) {
				nav_agent.isStopped = true;
				ti_charge.activate();
				OnStartCharge();
			}
		}

		UpdateTimers();
		HandleEvents();
	}

	public override void End() {
		// TODO: make this some point in the room
		nav_agent.destination = ghost.transform.position;
		nav_agent.stoppingDistance = 0;
		nav_agent.isStopped = false;
	}

	public override void OnEndCharge() {
		ghost.ChangeAnimation("Power_SlapLeft");
	}
	public override void OnEndActiveDelay() {

		float dist_to_gp = (this.ghost.transform.position - this.ghost.ghostPuncher.transform.position).magnitude;
		Debug.Log("Slap Dist: "+dist_to_gp);
		if (dist_to_gp <= attrs.SLAP_HIT_DISTANCE) {
			this.ghost.escape_meter += 10;
			this.ghost.ghostPuncher.GetComponent<GhostPuncher>().GetSlapped();
		}


	}

	public override void OnEndActive() {
	}

}

