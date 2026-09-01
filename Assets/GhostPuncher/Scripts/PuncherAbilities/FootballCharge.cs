using UnityEngine;

enum ChargePhase {
	// Little wind up before the charge
	STARTING,
	// Actively running
	CHARGING,
	// Coming to a stop after cancelling the charge
	STOPPING,
	// Finishing the charge with a punch
	PUNCHING
}

public class FootballCharge : PuncherAbility {

	Timer ti_charge;
	Timer ti_stop;
	Timer ti_punch_delay;
	Timer ti_punch_stop;
	float charge_speed;
	ChargePhase phase;
	FootballCollider charge_object;

	public FootballCharge(GhostPuncher p) : base(p) {
		lock_punch = true;

		phase = ChargePhase.STARTING;

		ti_charge = new Timer(puncher.defaults.CHARGE_CHARGE_TIME, puncher.defaults.CHARGE_CHARGE_TIME);
		ti_stop = new Timer(puncher.defaults.CHARGE_STOP_TIME, puncher.defaults.CHARGE_STOP_TIME);
		ti_punch_delay = new Timer(puncher.defaults.CHARGE_PUNCH_DELAY, puncher.defaults.CHARGE_PUNCH_DELAY);
		ti_punch_stop = new Timer(puncher.defaults.CHARGE_PUNCH_STOP, puncher.defaults.CHARGE_PUNCH_STOP);
		charge_speed = puncher.defaults.CHARGE_START_SPEED;

		charge_object = puncher.GetComponentInChildren<FootballCollider>(true);
		

	}

	public override void EnterAbility() {
		if (puncher.stamina <= 0) {
			// TODO: play an animation or something
			return;
		}

		ti_charge.Reset();
		ti_stop.Reset();
		ti_punch_delay.Reset();
		ti_punch_stop.Reset();

		charge_speed = puncher.defaults.CHARGE_START_SPEED;

		puncher.ChangeAnimation("ARM_TACKLE_START");
		charge_object.end_charge = false;
		phase = ChargePhase.STARTING;

	}

	public override void Update() {

		if (phase == ChargePhase.STARTING) {
			Update_Starting();
		} else if (phase == ChargePhase.CHARGING) {
			Update_Charging();
		} else if (phase == ChargePhase.STOPPING) {
			Update_Stopping();
		} else if (phase == ChargePhase.PUNCHING) {
			Update_Punching();
		}

	}

	void Update_Starting() {
		ti_charge.Tick(Time.deltaTime);

		if (ti_charge.FinishedThisFrame()) {
			puncher.look_damping_left = puncher.defaults.CHARGE_LOOK_LEFT_RIGHT_DAMPING;
			puncher.look_damping_right = puncher.defaults.CHARGE_LOOK_LEFT_RIGHT_DAMPING;
			puncher.look_damping_down = puncher.defaults.CHARGE_LOOK_UP_DOWN_DAMPING;
			puncher.look_damping_up = puncher.defaults.CHARGE_LOOK_UP_DOWN_DAMPING;
			puncher.move_damping_left = puncher.defaults.CHARGE_MOVE_LEFT_RIGHT_DAMPING;
			puncher.move_damping_right = puncher.defaults.CHARGE_MOVE_LEFT_RIGHT_DAMPING;
			puncher.move_damping_forward = 0;
			puncher.move_damping_back = 0;

			charge_object.gameObject.SetActive(true);
			phase = ChargePhase.CHARGING;
		}
	}

	void Update_Charging() {
		puncher.SpendStamina(puncher.defaults.CHARGE_STAMINA_DRAIN * Time.deltaTime);

		/* Controls */
		Vector2 move_value = puncher.action_move.ReadValue<Vector2>();
		if (move_value.y < 0) {
			// We tried to move backwards, so cancel the charge
			phase = ChargePhase.STOPPING;
			return;
		}

		if (puncher.action_chargePunch.WasPerformedThisFrame()) {
			// TODO: maybe play some special animation, like getting ready to punch?
			// Alternatively, this could just activate the punch
			puncher.ChangeAnimation("ARM_CHARGE_WINDUP");
		}

		if (puncher.action_attack.WasPerformedThisFrame()) {
			phase = ChargePhase.PUNCHING;
			return;
		}

		// Acceleration needs an FOV effect
		charge_speed += puncher.defaults.CHARGE_ACCELERATION * Time.deltaTime;
		if (charge_speed >= puncher.defaults.CHARGE_MAX_SPEED) { charge_speed = puncher.defaults.CHARGE_MAX_SPEED; }

		// Stamina drain while charging
		if (puncher.stamina <= 0) {
			phase = ChargePhase.STOPPING;
			return;
		}

		// Query charge object to see if we hit something that stops the charge
		if (charge_object.end_charge) {
			SlamToAStop();
			return;
		}
	}

	void SlamToAStop() {
		puncher.ChangeAnimation("ARM_TACKLE_END");
		puncher.ExitAbility();
	}

	void Update_Stopping() {
		ti_stop.Tick(Time.deltaTime);
		// This might cause a little bump up in speed as we stop, but I think I'm okay with that
		charge_speed = (puncher.defaults.CHARGE_MAX_SPEED*0.8f) * ti_stop.GetLerped(LerpTypes.EASE_OUT);

		if (ti_stop.Finished()) {
			puncher.ChangeAnimation("ArmIdle");
			puncher.ExitAbility();
		}
	}

	void Update_Punching() {
		ti_punch_delay.Tick(Time.deltaTime);

		if (ti_punch_delay.FinishedThisFrame() || ti_punch_delay.default_time == 0) {
			puncher.ChangeAnimation("CHARGE_PUNCH");
			// TODO: launch a punch
		}

		if (!ti_punch_delay.Finished()) {
			return;
		}

		ti_punch_stop.Tick(Time.deltaTime);
		charge_speed = (puncher.defaults.CHARGE_MAX_SPEED*0.8f) * ti_punch_stop.GetLerped(LerpTypes.EASE_OUT);
		if (ti_punch_stop.FinishedThisFrame()) {
			puncher.ExitAbility();
		}

		

	}

	public override Vector3 GetDesiredControlVec(Vector3 desired_vec) { 
		if (ti_charge.Finished()) {
			Vector3 look_dir = puncher.GetFacingDirection();
			look_dir.y = 0;
			return desired_vec + look_dir * charge_speed;
		} else {
			return desired_vec;
		}
	}

	public override void ExitAbility() {
		charge_object.gameObject.SetActive(false);

		// Reset damping
		puncher.look_damping_left = 1;
		puncher.look_damping_right = 1;
		puncher.look_damping_up = 1;
		puncher.look_damping_down = 1;

		puncher.move_damping_left = 1;
		puncher.move_damping_right = 1;
		puncher.move_damping_forward = 1;
		puncher.move_damping_back = 1;
	}

}
