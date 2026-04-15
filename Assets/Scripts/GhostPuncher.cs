using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public struct Punch {
	public Punch(Vector3 direction, float force, float object_damage, float ghost_damage, float poise_damage, int hitClass) {
		Direction = direction;
		Force = force;
		ObjectDamage = object_damage;
		GhostDamage = ghost_damage;
		PoiseDamage = poise_damage;
		HitClass = hitClass;
	}
	public Vector3 Direction;
	public float Force;
	public float ObjectDamage;
	public float PoiseDamage;
	public float GhostDamage;
	// 1st class punch is the strongest, 2nd class is a normal punch, 3 is big object, 4 is light object
	public int HitClass;
};



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

	const float PUNCH_RANGE = 3;

	public PuncherDefaults defaults; 
	public GhostPowerAttribs power_attribs;

	string punch_with = "RIGHT";
	bool buffered_punch = false;
	bool buffered_charge = false;

	Animator arm_animator;

	Vector3 push_dir;
	float push_power;
	float push_power_decay = 25;

	float speed_penalty;
	Timer ti_slowed;

	List<StatusEffect> statuses = new List<StatusEffect>();

	// UI Control Vars. That is - cleared or manipulated by UI ONLY!
	public bool uiFlag_slapped_this_frame;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		action_chargePunch = InputSystem.actions.FindAction("ChargePunch");
		action_attack = InputSystem.actions.FindAction("Attack");
		action_move = InputSystem.actions.FindAction("Move");

		arm_animator = this.GetComponentInChildren<Animator>();

		move_speed = defaults.MOVE_SPEED;

		layer_punchable = LayerMask.GetMask("Punchable");

		controller = GetComponent<CharacterController>();

		// Init Timers
		ti_punch_cooldown = new Timer(0, defaults.PUNCH_COOLDOWN);
		ti_punch_again = new Timer(0, defaults.PUNCH_COOLDOWN + defaults.PUNCH_AGAIN);
		ti_charge_up = new Timer(0, 0.5f);
		ti_charge_up.deactivate();

		speed_penalty = 0;
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
				ChangeAnimation("ARM_CHARGE_WINDUP");
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
					DoMegaPunch();
				} else {
					DoPunch();
				}

				ti_charge_up.deactivate();
				ti_charge_up.reset();
			}
		}

		// Moving
		Vector3 move_vec = controller.isGrounded ? new Vector3(0, 0, 0) : Physics.gravity;
		Vector3 desired_control_vec = moveControls();

		float speed_multiplier = 1 - GetSlowMultiplier();

		desired_control_vec *= speed_multiplier;

		move_vec += desired_control_vec;

		if (desired_control_vec.magnitude > 0) {
			if (!arm_animator.GetBool("Walking")) {
				PlayAnimation("Walk", 1);
				arm_animator.SetBool("Walking", true);
			}
		} else if (arm_animator.GetBool("Walking")) {
			StopAnimation(1);
			arm_animator.SetBool("Walking", false);
		}


		if (push_power > 0) {
			move_vec += push_dir * push_power;
			// There is probably a better way of making the push ease out
			if (push_power < 0.5) {
				push_power *= power_attribs.WAVE_DECAY / 1.5f;
			} else {
				push_power *= power_attribs.WAVE_DECAY;
			}
			if (push_power < power_attribs.WAVE_POWER_THRESHOLD) { push_power = 0; }
		}

		controller.Move(move_vec * Time.deltaTime);

		//controller.move(move_vec);


	}

	void DoPunch() {
		ChangeAnimation("PUNCH_"+punch_with);
		ExecutePunch(defaults.PUNCH_FORCE, defaults.PUNCH_OBJECT_DAMAGE, defaults.PUNCH_GHOST_DAMAGE, defaults.PUNCH_POISE_DAMAGE, 2);
	}

	void DoMegaPunch() {
		ChangeAnimation("CHARGE_PUNCH");
		ExecutePunch(defaults.MEGAPUNCH_FORCE, defaults.MEGAPUNCH_OBJECT_DAMAGE, defaults.MEGAPUNCH_GHOST_DAMAGE, defaults.MEGAPUNCH_POISE_DAMAGE, 1);
	}

	void ExecutePunch(float force, float object_damage, float ghost_damage, float poise_damage, int hitClass) {

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

			Punch punch = new Punch(ray_dir, force, object_damage, ghost_damage, poise_damage, hitClass);

			if (hit_col.CompareTag("BreakableObject")) {
				BreakableObject bo = hit_col.gameObject.GetComponent<BreakableObject>();
				bo.GetPunched(punch, attack_hit.point);

			} else if (hit_col.CompareTag("Ghost") || hit_col.CompareTag("GhostBodyCollider")) {
				Ghost g = hit_col.gameObject.GetComponent<Ghost>();
				if (!g) {
					g = hit_col.gameObject.GetComponentInParent<Ghost>();
				}
				g.GetPunched(punch);
				ectoplasm += 5;
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

	
		for (int i=statuses.Count-1; i>=0; i--) {
			statuses[i].Duration.tick(Time.deltaTime);
			if (statuses[i].Duration.finished()) {
				statuses.RemoveAt(i);
			}
		}
	}

	void ChangeAnimation(string name, float fade=0) {
		arm_animator.CrossFade(name, fade);
	}

	/** Useful for layering anims together **/
	void PlayAnimation(string name, int layer=-1) {
		arm_animator.Play(name, layer, 0.0f);
	}

	void StopAnimation(int layer, float fade_time=0.25f, string stopAnimName="Stop") {
		arm_animator.CrossFade(stopAnimName, fade_time, layer);
	}


	/** EVENTS **/
	public void GetPushed(Vector3 dir, float power) {
			push_dir = dir.normalized;
			push_power = power;
	}

	public void GetSlapped() {
		// Used by UI
		uiFlag_slapped_this_frame = true;
	}

	public void AddStatus(StatusEffect new_status) {
		statuses.Add(new_status);
	}

	/** STATUS **/
	float GetSlowMultiplier() {
		float total_slow_multiplier = 0;

		for (int i=statuses.Count-1; i>=0; i--) {
			if (statuses[i].Type == StatusType.SLOWED) {
				total_slow_multiplier += statuses[i].GetFloatValue(StatusAttribs.SLOWED_STRENGTH);
			}
		}

		return total_slow_multiplier;
	}
}


