using UnityEngine;

enum ChargingEscapePhase {
	MOVE,
	CHARGE,
}

public class GA_POW_ChargingEscape : GhostAction {

	ChargingEscapePhase phase;

	Transform move_target;

	public GA_ChargingEscape(Ghost g) : base(g) {}

	public override void Enter() {
		phase = ChargingEscapePhase.MOVE;
		Enter_Move();
	}

	public override void Exit() {
		ghost.charge_particles.Stop();
	}

	public override void Update() {
		Update_Charge();
	}

	/** Sub-states **/

	// Move
	public void Enter_Move() {
		// TODO: Choose a spot to go
	}

	public void Update_Move() {
		// TODO: If close to move target, start charging
	}
	
	// Charge
	public Enter_Charge() {
				// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
				ghost.charge_particles.Play();
				ghost.ChangeAnimation("ChargeEscape");
				ghost.PlaySound("charging_escape");
	}

	public Update_Charge() {
		// TODO: be making wubwubwubwubwubwubwub sound
		ghost.escape_meter += Time.deltaTime;

		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}
	
}
