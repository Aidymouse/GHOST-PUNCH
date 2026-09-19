using UnityEngine;

enum ChargingEscapePhase {
	MOVE,
	CHARGE,
}

public class GA_POW_ChargingEscape : GhostAction {

	ChargingEscapePhase phase;

	Transform move_target;

	public GA_POW_ChargingEscape(Ghost g) : base(g) {}

	public override void Enter() {
		phase = ChargingEscapePhase.MOVE;
		ghost.nav_agent.isStopped = false;
		Enter_Move();
	}

	public override void Exit() {
		ghost.charge_particles.Stop();
	}

	public override void Update() {
		if (phase == ChargingEscapePhase.MOVE) {
			Update_Move();
		} else if (phase == ChargingEscapePhase.CHARGE) {
			Update_Charge();
		}
	}

	/** Sub-states **/

	// Move
	public void Enter_Move() {
		// TODO: Choose a spot to go
		GameObject[] destinations = GameObject.FindGameObjectsWithTag("GhostDestination"); // Supposedly slow, but shouldn't be a big deal
		int dest_idx = Random.Range(0, destinations.Length);
		GameObject dest_obj = destinations[dest_idx];

		ghost.nav_agent.destination = dest_obj.transform.position;
		//if (debug) { Debug.Log("[MovingRoom] new dest"+dest_obj.transform.position); }
		ghost.nav_destination = dest_obj;
	}

	public void Update_Move() {
		// TODO: If close to move target, start charging
		float dist_to_dest = GhostUtils.TopDownDistance(ghost.transform.position, ghost.nav_destination.transform.position);
//(ghost.transform.position - ghost.nav_destination.transform.position).magnitude;
		if (dist_to_dest <= 1) {
			ghost.nav_destination = null;
			Enter_Charge();
		}
	}

	// Charge
	public void Enter_Charge() {
		// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
		ghost.charge_particles.Play();
		ghost.ChangeAnimation("ChargeEscape");
		ghost.PlaySound("charging_escape");
		phase = ChargingEscapePhase.CHARGE;
	}

	public void Update_Charge() {
		// TODO: be making wubwubwubwubwubwubwub sound
		ghost.escape_meter += Time.deltaTime;

		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}

}
