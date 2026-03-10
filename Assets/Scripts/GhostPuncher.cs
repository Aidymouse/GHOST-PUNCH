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

		// Animation State
		if (ti_punch_again.finished_this_frame) {
			arm_animator.SetBool("PunchL", false);
		}

		// Attacking

		if (action_chargePunch.WasPerformedThisFrame()) {
		  arm_animator.SetTrigger("StartChargingPunch");
		  arm_animator.SetBool("ChargingPunch", true);
		  ti_charge_up.activate();
		  ti_charge_up.reset();
		}

		if (action_attack.WasPerformedThisFrame()) {
		  if (ti_charge_up.finished()) {
		    doMegaPunch();
		    ti_charge_up.deactivate();
		  } else {
		    arm_animator.SetBool("ChargingPunch", false);
		    doPunch();
		  }
		}

		// Moving
		moveControls();

	}

	void doMegaPunch() {
	  Debug.Log("Mega Punch!");
	  arm_animator.SetTrigger("DoPunch");
	  //arm_animator.SetBool("ChargingPunch", false);
	  // TODO
	}

	void doPunch() {

		if (!ti_punch_cooldown.finished()) {
			return;
		}

		ti_punch_cooldown.reset();	
		ti_punch_again.reset();	

		// Animator
		Animator arm_animator = this.GetComponentInChildren<Animator>();

		if (!ti_punch_again.finished()) {
			bool punch_left = arm_animator.GetBool("PunchL");
			arm_animator.SetBool("PunchL", !punch_left);
		}

		arm_animator.SetTrigger("DoPunch");

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

				Debug.Log("Punching Object!");

				BreakableObject bo = hit_col.gameObject.GetComponent<BreakableObject>();
				bo.GetPunched(1, ray_dir);

				//Destroy(hit_col);

			} else if (hit_col.CompareTag("PunchableObject")) {
			  Destroy(hit_col.gameObject);
			  // TODO: particles or some objs have HP or smn

			  // GameObject punchable = hit_col.gameObject;
			  // Rigidbody rb = punchable.GetComponent<Rigidbody>();
			  //
			  // if (rb) {
			  //   Vector3 blast_dir = ray_dir;
			  //   rb.isKinematic = false;
			  //   rb.AddForce(blast_dir.normalized * 200);
			  // }

			} else if (hit_col.CompareTag("Ghost")) {
				Ghost g = hit_col.gameObject.GetComponent<Ghost>();
				g.GetPunched();
				ectoplasm += 5;
				ui.UpdateEctoplasm(ectoplasm);
			}



		}

	}

	void moveControls() {
		Vector2 move_value = action_move.ReadValue<Vector2>();
		if (move_value.x == 0 && move_value.y == 0) { return; }

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

		controller.Move(movement * move_speed * Time.deltaTime);
	}

	void tick_timers() {
		ti_punch_cooldown.tick(Time.deltaTime);
		ti_punch_again.tick(Time.deltaTime);
		ti_charge_up.tick(Time.deltaTime);
	}

}


