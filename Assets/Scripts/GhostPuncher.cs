using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPuncher : MonoBehaviour
{
	InputAction action_attack;
	InputAction action_move;
	InputAction action_chargePunch;

	CharacterController controller;

	Timer ti_punch_cooldown;
	Timer ti_punch_again;
	Timer ti_charge_up;

	public GhostUI ui;
	public DebugUI debug_ui;

	float move_speed;

	int ectoplasm = 0;

	LayerMask layer_punchable;

	const float PUNCH_RANGE = 2;

	public PuncherDefaults defaults; 

	string punch_with = "RIGHT";
	bool buffered_punch = false;
	bool buffered_charge = false;

	Animator arm_animator;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		action_chargePunch = InputSystem.actions.FindAction("ChargePunch");
		action_attack = InputSystem.actions.FindAction("Attack");
		action_move = InputSystem.actions.FindAction("Move");

		arm_animator = this.GetComponentInChildren<Animator>();

		move_speed = 5;

		layer_punchable = LayerMask.GetMask("Punchable");

		controller = GetComponent<CharacterController>();

		// Init Timers
		ti_punch_cooldown = new Timer(0, defaults.PUNCH_COOLDOWN);
		ti_punch_again = new Timer(0, defaults.PUNCH_COOLDOWN + defaults.PUNCH_AGAIN);
		ti_charge_up = new Timer(0, 0.5f);
		ti_charge_up.deactivate();

	}

	// Update is called once per frame
	void Update()
	{
		// Timers
		this.tick_timers();

		// Attacking
		if (ti_punch_again.finished_this_frame()) {
			punch_with = "RIGHT";
		}

		if ((action_chargePunch.WasPerformedThisFrame() && !buffered_punch) || (buffered_charge && ti_punch_cooldown.finished_this_frame())) {

			buffered_charge = true;

			if (ti_punch_cooldown.finished()) {
				ti_charge_up.activate();
				ti_charge_up.reset();
				change_anim("ARM_CHARGE_WINDUP");
			}

		}

		if (action_attack.WasPerformedThisFrame() || (buffered_punch && ti_punch_cooldown.finished_this_frame())) {

			buffered_charge = false;

			if (ti_punch_cooldown.time_remaining < defaults.PUNCH_BUFFER_TIME) {
				// This gets set even on successful punch, but doesn't matter cos it'll get unset when we punch
				buffered_punch = true; 
			}

			if (ti_punch_cooldown.finished()) {

				buffered_punch = false;

				if (!ti_punch_again.finished()) {
					punch_with = punch_with == "RIGHT" ? "LEFT" : "RIGHT";
				} 

				ti_punch_cooldown.reset();	
				ti_punch_again.reset();	

				if (ti_charge_up.finished()) {
					doMegaPunch();
				} else {
					doPunch(defaults.PUNCH_POWER);
				}

				ti_charge_up.deactivate();
				ti_charge_up.reset();
			}
		}

		// Moving
		Vector3 move_vec = controller.isGrounded ? new Vector3(0, 0, 0) : Physics.gravity;
		move_vec += moveControls();

		controller.Move(move_vec * Time.deltaTime);

		//controller.move(move_vec);


	}

	void doMegaPunch() {
		doPunch(defaults.PUNCH_MEGA_POWER);
	}


	void doPunch(float power) {

		change_anim("PUNCH_"+punch_with);

		// Cast a ray - jeff says should be a box
		RaycastHit attack_hit;

		Camera cam = this.GetComponentInChildren<Camera>();

		//Vector3 ray_dir = transform.TransformDirection(Vector3.forward);
		Vector3 ray_dir = cam.transform.TransformDirection(Vector3.forward);

		if (Physics.Raycast(cam.transform.position, ray_dir, out attack_hit, PUNCH_RANGE, layer_punchable)) {
			Debug.DrawRay(transform.position, ray_dir, Color.red, 1, false);

			Collider hit_col = attack_hit.collider;

			if (hit_col == null) {
				return;
			}

			if (hit_col.CompareTag("BreakableObject")) {

				BreakableObject bo = hit_col.gameObject.GetComponent<BreakableObject>();
				bo.GetPunched(power, ray_dir, attack_hit);

			} else if (hit_col.CompareTag("Ghost")) {
				Ghost g = hit_col.gameObject.GetComponent<Ghost>();
				g.GetPunched(power);
				ectoplasm += 5;
				ui.UpdateEctoplasm(ectoplasm);
			}



		}

	}

	Vector3 moveControls() {

		Vector2 move_value = action_move.ReadValue<Vector2>();
		if (move_value.x == 0 && move_value.y == 0) { return new Vector3(0, 0, 0); }

		Vector3 movement_frontback = new Vector3(0, 0, 0);
		Vector3 movement_horiz = new Vector3(0, 0, 0);

		if (move_value.x > 0) {
			movement_horiz = transform.TransformDirection(Vector3.right);
		} else if (move_value.x < 0) {
			movement_horiz = transform.TransformDirection(Vector3.left);
		}

		if (move_value.y > 0) {
			movement_frontback = transform.TransformDirection(Vector3.forward);
		} else if (move_value.y < 0) {
			movement_frontback = transform.TransformDirection(Vector3.back);
		}

		Vector3 movement = movement_frontback + movement_horiz;
		movement.y = 0;
		movement = movement.normalized;

		Vector3 move_vec = movement * move_speed; // * Time.deltaTime;

		return move_vec;
	}

	void tick_timers() {
		ti_punch_cooldown.tick(Time.deltaTime);
		ti_punch_again.tick(Time.deltaTime);
		ti_charge_up.tick(Time.deltaTime);
	}

	void change_anim(string name, float fade=0) {
		arm_animator.CrossFade(name, fade);
	}

}


