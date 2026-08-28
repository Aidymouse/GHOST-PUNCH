using UnityEngine;

public class FootballCharge : PuncherAbility {

	Timer ti_charge;
	/* TODO: make charge end when something is hit rather than  */
	Timer ti_temp;
	float charge_speed;

	//GameObject charge_object;
	FootballCollider charge_object;

	public FootballCharge(GhostPuncher p) : base(p) {
		ti_charge = new Timer(1, 1);
		ti_temp = new Timer(2, 2);
		charge_speed = puncher.defaults.CHARGE_START_SPEED;

		charge_object = puncher.GetComponentInChildren<FootballCollider>(true);
		
	}

	public override void EnterAbility() {
		if (puncher.stamina <= 0) {
			// TODO: play an animation or something
			return;
		}

		ti_charge.Reset();
		ti_temp.Reset();
		puncher.ChangeAnimation("ARM_TACKLE_START");
		charge_object.end_charge = false;
	
	}

	public override void Update() {
		
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
			}

			if (ti_charge.Finished()) {
				ti_temp.Tick(Time.deltaTime);
				puncher.SpendStamina(puncher.defaults.CHARGE_STAMINA_DRAIN * Time.deltaTime);
			}

			if (ti_temp.Finished()) {
				puncher.ExitAbility();
				return;
			}

			// Acceleration needs an FOV effect
			charge_speed += puncher.defaults.CHARGE_ACCELERATION * Time.deltaTime;
			if (charge_speed >= puncher.defaults.CHARGE_MAX_SPEED) { charge_speed = puncher.defaults.CHARGE_MAX_SPEED; }

			// Stamina drain while charging
			if (puncher.stamina <= 0) {
				puncher.ExitAbility();
				return;
			}

			if (charge_object.end_charge) {
				puncher.ExitAbility();
				return;
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

		puncher.ChangeAnimation("ARM_TACKLE_END");

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
