using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum HitClass {
	MEGA_PUNCH=0,
	LARGE_ITEM=1,
	PUNCH=2,
	ITEM=3,
}

public enum PuncherAbilites {
	FOOTBALL_CHARGE=0,
}

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

/* The hit record is passed around as we execute punches, then taken by the ghost puncher and assessed to see what kind of bonuses we get */
public struct PunchRecord {
	public int items_hit;
	public int items_broken;
	public bool hit_ghost;
	public bool ragdolled_ghost;
}


public class GhostPuncher : MonoBehaviour
{

	[Tooltip("If true, the puncher spawns in playable form, rather than being dormant like for the main game.")]
	public bool start_active;	

	InputAction action_attack;
	InputAction action_move;
	InputAction action_chargePunch;
	InputAction action_ability1;
	InputAction action_ability2;
	InputAction action_ability3;

	CharacterController controller;
	float move_speed;

	public PuncherDefaults defaults; 
	public GhostPowerAttribs power_attribs;

	float fall_velocity;

	/* Stamina */
	[HideInInspector]
	public float max_stamina;
	public float stamina;
	float stamina_recharge_rate;
	Timer ti_stamina_recharge;

	/* Punch */
	Timer ti_punch_cooldown;
	Timer ti_punch_again;
	Timer ti_charge_up;
	float punch_range;
	string punch_with = "Right";
	bool buffered_punch = false;
	bool buffered_charge = false;
	bool charging_punch = false;

	/* Fear Meter */
	// Indexes into the fear_multipliers and fear_required lists of defaults
	[HideInInspector] public int fear_index;
	[HideInInspector] public int max_fear_index;
	[HideInInspector] public float fear_multiplier;
	[HideInInspector] public float fear_meter;
	public Timer ti_fear_reset;

	public AudioSource footstepSound;
	public float pitchLow;
	public float pitchHigh;

	[Header("Footsteps")]
	public AudioClip footSound1;
	public AudioClip footSound2;
	public float stepCooldown;
	Timer ti_step_sound;

	private float stepRate;
	private bool isMoving;

	/* Abilities */
	PuncherAbility?[] equipped_abilities;
	PuncherAbility? active_ability = null;

	/* Other */
	int ectoplasm = 0;

	// layers that are punchable.
	public LayerMask punchables_mask;
	// prefab box used as collider for punches
	public BoxCollider punch_hitbox;


	public Animator arm_animator;

	/** Camera effects **/
	// ??
	FOVKick fovKick;
	// ??
	ScreenShake screenShake;
	// Multipliers on looking around, applied in CameraController
	[HideInInspector] public float look_damping_left;
	[HideInInspector] public float look_damping_right;
	[HideInInspector] public float look_damping_up;
	[HideInInspector] public float look_damping_down;
	// Multipliers on moving around
	[HideInInspector] public float move_damping_left;
	[HideInInspector] public float move_damping_right;
	[HideInInspector] public float move_damping_forward;
	[HideInInspector] public float move_damping_back;

	/* Cutscene control toggle */
	public bool inCutscene = false;

	// TODO: this could totally be a status effect
	Vector3 push_dir;
	float push_power;
	float push_power_decay = 25;

	List<StatusEffect> statuses = new List<StatusEffect>();
	[Tooltip("The position in the list corresponds to the hit class. Slot 1 = hit class 1 (strong punch), etc.")]
	public List<ParticleSystem> punch_particles = new List<ParticleSystem>();

	// UI Control Vars. That is - cleared or manipulated by UI ONLY!
	[HideInInspector]
	public bool uiFlag_slapped_this_frame;
	[HideInInspector]
	public bool uiFlag_slowed;

	/** Locks - control so external states can manipulate ghost punchers abilities **/
	// Stops the directional move controls, but not looking around
	public bool lock_move_controls;

	void Start()
	{
		action_chargePunch = InputSystem.actions.FindAction("ChargePunch");
		action_attack = InputSystem.actions.FindAction("Attack");
		action_move = InputSystem.actions.FindAction("Move");
		action_ability1 = InputSystem.actions.FindAction("Ability1");
		action_ability2 = InputSystem.actions.FindAction("Ability2");
		action_ability3 = InputSystem.actions.FindAction("Ability3");

		fall_velocity = 0;

		//arm_animator = this.GetComponentInChildren<Animator>();
		//punchables_mask = LayerMask.GetMask("Punchable");

		/* Load Defaults */
		move_speed = defaults.MOVE_SPEED;
		// TODO: handle via items
		Debug.Log("Setting max stamina to "+defaults.BASE_STAMINA);
		max_stamina = defaults.BASE_STAMINA;
		stamina = max_stamina;
		stamina_recharge_rate = defaults.STAMINA_RECHARGE_RATE;
		punch_range = defaults.PUNCH_RANGE;

		/* Camera effects */
		fovKick = GetComponentInChildren<FOVKick>();
		screenShake = GetComponentInChildren<ScreenShake>();

		controller = GetComponent<CharacterController>();

		// Init Timers
		ti_punch_cooldown = new Timer(0, defaults.PUNCH_COOLDOWN);
		ti_punch_again = new Timer(0, defaults.PUNCH_COOLDOWN + defaults.PUNCH_AGAIN);
		ti_charge_up = new Timer(0, 0.5f);
		ti_charge_up.Deactivate();
		ti_stamina_recharge = new Timer(0, defaults.STAMINA_RECHARGE_DELAY);

		footstepSound = GetComponent<AudioSource>();
		footstepSound.clip = footSound1;
		stepRate = stepCooldown;

		if (!start_active) {
			Debug.Log("Ghost Puncher is going dormant.");
			GetComponentInChildren<CameraController>().enabled = false;
			Cursor.lockState = CursorLockMode.None;

			if (inCutscene == false) {
				Debug.LogWarning("Ghost puncher defying desired cutscene state because we started active");
			}
			inCutscene = true;
		}

		// Init Fear
		ti_fear_reset = new Timer(0, defaults.FEAR_RESET_TIMERS[0]); // this is a variable timer...
		fear_index = 0;
		max_fear_index = 3;

		// Init mouse damping
		look_damping_left = 1;
		look_damping_right = 1;
		look_damping_up = 1;
		look_damping_down = 1;
		move_damping_left = 1;
		move_damping_right = 1;
		move_damping_forward = 1;
		move_damping_back = 1;

		// Init Locks
		lock_move_controls = false;

		// Init abilities
		equipped_abilities = new PuncherAbility?[3];
		equipped_abilities[0] = new FootballCharge(this);
		equipped_abilities[1] = null;
		equipped_abilities[2] = null;

	}



	// Update is called once per frame
	void Update()
	{
		if (inCutscene)	{
			return;
		}

		// Timers
		this.tick_timers();

		UpdatePunch();

		UpdateFearMeter();

		// Abilites
		if (action_ability1.WasPerformedThisFrame() && equipped_abilities[0] is not null) {
			
			active_ability = equipped_abilities[0];
			active_ability.EnterAbility();

		}

		if (active_ability is not null) {
			active_ability.Update();
		}
		

		// Moving
		Vector3 desired_control_vec = new Vector3(0, 0, 0); 
		if (lock_move_controls == false) {
			desired_control_vec = moveControls();
		}
		if (active_ability is not null) {
			desired_control_vec = active_ability.GetDesiredControlVec(desired_control_vec);
		}

		HandleMove(desired_control_vec);


		// Stamina
		if (ti_stamina_recharge.Finished() && !charging_punch) {
			stamina += stamina_recharge_rate * Time.deltaTime;
			if (stamina > max_stamina) { stamina = max_stamina; }
		}

		//Footsteps
		HandleStepSounds();

	}

	public void ExitAbility() {
		if (active_ability is null) { return; }
		active_ability.ExitAbility();
		active_ability = null;
	}




	/** MOVEMENT METHODS **/
	void HandleMove(Vector3 desired_control_vec) {

		if (controller.isGrounded) {
			fall_velocity = 0;
		} else {
			fall_velocity += Physics.gravity.y * Time.deltaTime;
		}

		Vector3 move_vec = new Vector3(0, fall_velocity, 0);

		float speed_multiplier = 1 - GetSlowMultiplier();
		desired_control_vec *= speed_multiplier;
		move_vec += desired_control_vec;

		if (desired_control_vec.magnitude > 0) {
			if (!arm_animator.GetBool("Walking")) {
				PlayAnimation("Walk", 1);
				arm_animator.SetBool("Walking", true);
				isMoving = true;
			}
		} else if (arm_animator.GetBool("Walking")) {
			StopAnimation(1);
			arm_animator.SetBool("Walking", false);
			isMoving = false;
		}

		// Push
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

		// Execute the move
		controller.Move(move_vec * Time.deltaTime);
	}





	/** PUNCH METHODS **/
	void UpdatePunch() {
		if (ti_punch_again.FinishedThisFrame()) {
			punch_with = "Right";
		}

		if ((action_chargePunch.WasPerformedThisFrame() && !buffered_punch) || (buffered_charge && ti_punch_cooldown.FinishedThisFrame())) {

			buffered_charge = true;

			if (ti_punch_cooldown.Finished()) {
				ti_charge_up.Activate();
				ti_charge_up.Reset();
				ChangeAnimation("ARM_CHARGE_WINDUP");
				charging_punch = true;
			}

		}

		if (action_attack.WasPerformedThisFrame() || (buffered_punch && ti_punch_cooldown.FinishedThisFrame())) {

			buffered_charge = false;

			if (ti_punch_cooldown.time_remaining < defaults.PUNCH_BUFFER_TIME) {
				// This gets set even on successful punch, but doesn't matter cos it'll get unset when we punch
				buffered_punch = true; 
			}

			if (ti_punch_cooldown.Finished()) {

				buffered_punch = false;

				if (!ti_punch_again.Finished()) {
					punch_with = punch_with == "Right" ? "Left" : "Right";
				} 


				if (ti_charge_up.Finished() && stamina > 0) {
					// TODO: feebler animation if this happens
					DoMegaPunch();
					ti_punch_cooldown.Set(GetMegaPunchCooldown());	
				} else {
					DoPunch();
					ti_punch_cooldown.Set(GetPunchCooldown());	
					ti_punch_again.Reset();	
				}

				charging_punch = false;

				ti_charge_up.Deactivate();
				ti_charge_up.Reset();
			}
		}
	}

	void DoPunch() {
		int punch_num = Random.Range(1,5);
		ChangeAnimation("Jab"+punch_with+punch_num);

		if (fovKick) { fovKick.SmallKick(); }
		if (screenShake) { screenShake.Shake(0.05f); }
		Punch normal_punch = new Punch(
			punch_hitbox.transform.TransformDirection(Vector3.forward),
 			defaults.PUNCH_FORCE,
			defaults.PUNCH_OBJECT_DAMAGE,
			defaults.PUNCH_GHOST_DAMAGE,
			defaults.PUNCH_POISE_DAMAGE,
			2
		);

		PunchRecord record = ExecutePunch(normal_punch, defaults.PUNCH_STAMINA);

		AssessPunchRecord(record);
	}

	void DoMegaPunch() {
		ChangeAnimation("CHARGE_PUNCH");
		if (fovKick) fovKick.BigKick();
		if (screenShake) screenShake.Shake(0.2f);
		Punch mega_punch = new Punch(
			punch_hitbox.transform.TransformDirection(Vector3.forward),
			defaults.MEGAPUNCH_FORCE,
			defaults.MEGAPUNCH_OBJECT_DAMAGE,
			defaults.MEGAPUNCH_GHOST_DAMAGE,
			defaults.MEGAPUNCH_POISE_DAMAGE,
			1
		);

		PunchRecord mega_record = ExecutePunch(mega_punch, defaults.MEGAPUNCH_STAMINA);
		AssessPunchRecord(mega_record);
	}

	/** returns true if we hit something */
	PunchRecord ExecutePunch(Punch punch, float stamina_used) {

		PunchRecord record = new PunchRecord();
		Collider[] punched = Physics.OverlapBox(punch_hitbox.transform.position, punch_hitbox.transform.localScale/2, punch_hitbox.transform.rotation, punchables_mask);		

		SpendStamina(stamina_used);

		List<int> punched_ids = new List<int>();

		Vector3 look_dir = punch_hitbox.transform.TransformDirection(Vector3.forward);
		Vector3 look_start = punch_hitbox.transform.position - look_dir * punch_hitbox.transform.localScale.z/2;

		// cast a ray from look dir toward target
		//RaycastHit[] hits = Physics.RaycastAll(new Ray(look_start, look_dir), punch_hitbox.transform.localScale.z, punchables_mask);

		foreach (Collider col in punched) {
			RaycastHit? relevant_hit = null;

			//Punch punch_copy = punch; // copy?
			
			ProcessPunchTarget(col.gameObject, punch, punched_ids, ref record, relevant_hit);
		}

		return record;

	}

	void ProcessPunchTarget(GameObject target, Punch punch, List<int> punched_ids, ref PunchRecord record, RaycastHit? relevant_hit) {

		// May want to move this up later. Also, do we need to cast a ray to get the hit point for particles ??
		//if (punch.HitClass-1 < punch_particles.Count && punch_particles[hitClass-1]) {
			//Instantiate(punch_particles[punch.HitClass-1], attack_hit.point, this.transform.rotation);
		//}

		if (target.GetComponent<BreakableObject>()) {
			BreakableObject bo = target.GetComponent<BreakableObject>();
			int bo_id = bo.GetInstanceID();

			if (punched_ids.Contains(bo_id)) { return; }

			bo.GetPunched(punch);
			punched_ids.Add(bo_id);

			record.items_hit += 1;
		}

		Ghost ghost = target.GetComponent<Ghost>();
		if (!ghost) { ghost = target.GetComponentInParent<Ghost>(); }
		if (ghost) {
			int ghost_id = ghost.GetInstanceID();
			if (punched_ids.Contains(ghost_id)) { return; }
			ghost.GetPunched(punch);
			punched_ids.Add(ghost_id);

			record.hit_ghost = true;
		}
	}

	// After a punch is executed, assess the record to see what bonuses we get
	void AssessPunchRecord(PunchRecord record) {
		if (record.items_hit > 0) {
			stamina += defaults.STAMINA_GAINED_ON_HIT;
			if (this.fear_meter > 0 || this.fear_index != 0) {
				ti_fear_reset.Reset();
			}
		}

		if (record.hit_ghost) {
			if (this.fear_index < this.max_fear_index) {
				this.fear_meter += defaults.PUNCH_FEAR;
			}
			ti_fear_reset.Reset();
		}
	}


	/** MOVEMENT **/
	Vector3 moveControls() {

		Vector2 move_value = action_move.ReadValue<Vector2>();
		if (move_value.x == 0 && move_value.y == 0) { return new Vector3(0, 0, 0); }

		Vector3 movement_frontback = new Vector3(0, 0, 0);
		Vector3 movement_horiz = new Vector3(0, 0, 0);

		if (move_value.x > 0) {
			movement_horiz = transform.TransformDirection(Vector3.right) * move_damping_right;
		} else if (move_value.x < 0) {
			movement_horiz = transform.TransformDirection(Vector3.left) * move_damping_left;
		}

		if (move_value.y > 0) {
			movement_frontback = transform.TransformDirection(Vector3.forward) * move_damping_forward;
		} else if (move_value.y < 0) {
			movement_frontback = transform.TransformDirection(Vector3.back) * move_damping_back;
		}

		Vector3 movement = movement_frontback + movement_horiz;
		movement.y = 0;
		movement = movement.normalized;

		Vector3 move_vec = movement * move_speed; // * Time.deltaTime;

		return move_vec;
	}

	void tick_timers() {
		ti_punch_cooldown.Tick(Time.deltaTime);
		ti_punch_again.Tick(Time.deltaTime);
		ti_charge_up.Tick(Time.deltaTime);
		ti_stamina_recharge.Tick(Time.deltaTime);


		for (int i=statuses.Count-1; i>=0; i--) {
			statuses[i].Duration.Tick(Time.deltaTime);
			if (statuses[i].Duration.Finished()) {
				statuses.RemoveAt(i);
			}
		}
	}

	void UpdateFearMeter() {
		ti_fear_reset.Tick(Time.deltaTime);
		if (ti_fear_reset.Finished()) {
			this.fear_multiplier = 1;
			this.fear_meter = 0;
			this.fear_index = 0;
			this.ti_fear_reset.SetTime(0, defaults.FEAR_RESET_TIMERS[0]);
		}


		if (this.fear_meter >= GetFearRequired() && this.fear_index < this.max_fear_index) {
			this.fear_index += 1;
			this.ti_fear_reset.SetTime(defaults.FEAR_RESET_TIMERS[this.fear_index], defaults.FEAR_RESET_TIMERS[this.fear_index]);
			this.fear_meter = 0;
		}

	}

	
	// Get's the fear required for the next fear tier
	public float GetFearRequired() {
		if (this.fear_index < this.max_fear_index) {
			return defaults.FEAR_REQUIRED[this.fear_index+1];
		}
		return -1;
	}
 	
	public float GetFearMultiplier() {
		return defaults.FEAR_MULTIPLIERS[this.fear_index];
	}

	/** ANIMATION **/

	public void ChangeAnimation(string name, float fade=0) {
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
	public void SpendStamina(float stamina_used) {
		if (stamina_used == 0) { return; }
		ti_stamina_recharge.Reset();
		stamina -= stamina_used;
		if (stamina < 0) { stamina = 0; }
	}

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

	/* Update all the state needed when a run begins */
	public void StartRun() {
		GetComponentInChildren<CameraController>().enabled = true;
		arm_animator.gameObject.SetActive(true);
		inCutscene = false;
	}

	public void EndRun() {
		GetComponentInChildren<CameraController>().enabled = false;
		// TODO: make this a 'put arms away' animation
		arm_animator.gameObject.SetActive(true);
		inCutscene = true;

	}


	public void ApplyItems(ItemRecord record) {
		for (int i=0; i<record.items.Count; i++) {
			Item item = record.items[i];
			item.ApplyToGhostPuncher(this);
		}
	}
	

	/** Update FNs */
	void HandleStepSounds() {
		if (isMoving == true && stepCooldown < 0f) {
			if (footstepSound.clip = footSound1) { footstepSound.clip = footSound2; }
			if (footstepSound.clip = footSound2) { footstepSound.clip = footSound1; }
			footstepSound.pitch = (Random.Range(pitchLow, pitchHigh));
			footstepSound.Play();
			stepCooldown = stepRate;
		}
		stepCooldown -= Time.deltaTime;
	}

	public Vector3 GetFacingDirection() {
			Vector3 look_dir = punch_hitbox.transform.TransformDirection(Vector3.forward);
			return look_dir;
	}


	/** Variable Data **/
	float GetPunchCooldown() {
		return defaults.PUNCH_COOLDOWN;
	}

	float GetMegaPunchCooldown() {
		return defaults.MEGAPUNCH_COOLDOWN;
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


