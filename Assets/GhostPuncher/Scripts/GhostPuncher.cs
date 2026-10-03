using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum HitClass {
	MEGA_PUNCH=0,
	LARGE_ITEM=1,
	PUNCH=2,
	ITEM=3,
}

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

	[HideInInspector] public InputAction action_attack;
	[HideInInspector] public InputAction action_move;
	[HideInInspector] public InputAction action_chargePunch;
	InputAction action_ability1;
	InputAction action_ability2;
	InputAction action_ability3;

	CharacterController controller;
	float move_speed;
	[HideInInspector] public FOVController fov_controller;

	public PuncherDefaults defaults; 
	public GhostDefaults ghost_defaults;

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
	// Fear drain in percent per second
	[HideInInspector] public float fear_drain;
	[HideInInspector] public float fear_multiplier;
	[HideInInspector] public float fear_meter;
	// Pauses fear drain
	public Timer ti_fear_drain_pause;
	public Timer ti_fear_last_chance;

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

	/** Puncher doesn't really own this animator, he just gets executive control of it during gameplay. Punchers arms actually live in the camera and sometimes get externally controlled (like for escape). */
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

	[Header("Throwing")]
	public GameObject held_object;
	public Transform throw_point;
	[Tooltip("The parent bone for held objects")] public Transform throw_parent;

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
		fov_controller = GetComponentInChildren<FOVController>();

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
			GoDormant();
			ChangeAnimation("KickedOutEnd");
		}

		// Init Fear
		fear_index = 0;
		max_fear_index = defaults.FEAR_MULTIPLIERS.Count;
		ti_fear_drain_pause = new Timer(defaults.FEAR_DRAIN_PAUSE, defaults.FEAR_DRAIN_PAUSE);
		ti_fear_last_chance = new Timer(defaults.FEAR_LAST_CHANCE, defaults.FEAR_LAST_CHANCE);
		fear_drain = defaults.FEAR_DRAIN;

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
		equipped_abilities[1] = new Throw(this);
		equipped_abilities[2] = new Drink(this);

	}



	// Update is called once per frame
	void Update()
	{

		// Timers
		this.tick_timers();

		
		if (active_ability is null || active_ability.lock_punch == false) {
			UpdatePunch();
		}

		UpdateFearMeter();

		// Abilites
		if (active_ability is null) {
			if (action_ability1.WasPerformedThisFrame() && equipped_abilities[0] is not null) {
				active_ability = equipped_abilities[0];
				active_ability.EnterAbility();
			} else if (action_ability2.WasPerformedThisFrame() && equipped_abilities[1] is not null) {
				active_ability = equipped_abilities[1];
				active_ability.EnterAbility();
			} else if (action_ability3.WasPerformedThisFrame() && equipped_abilities[2] is not null) {
				active_ability = equipped_abilities[2];
				active_ability.EnterAbility();
			}
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
				push_power *= ghost_defaults.blast_data.DECAY / 1.5f;
			} else {
				push_power *= ghost_defaults.blast_data.DECAY;
			}
			if (push_power < ghost_defaults.blast_data.POWER_THRESHOLD) { push_power = 0; }
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
					NormalPunch();
					ti_punch_cooldown.Set(GetPunchCooldown());	
					ti_punch_again.Reset();	
				}

				charging_punch = false;

				ti_charge_up.Deactivate();
				ti_charge_up.Reset();
			}
		}
	}


	void NormalPunch() {
		int punch_num = Random.Range(1,5);
		ChangeAnimation("Jab"+punch_with+punch_num);

		Punch normal_punch = Punch.FromData(
			punch_hitbox.transform.TransformDirection(Vector3.forward),
			defaults.NORMAL_PUNCH_DATA
		);

		LaunchPunch(normal_punch, defaults.PUNCH_STAMINA);
	}

	void DoMegaPunch() {
		ChangeAnimation("CHARGE_PUNCH");
		if (fovKick) fovKick.BigKick();
		if (screenShake) screenShake.Shake(0.2f);
		Punch mega_punch = Punch.FromData(punch_hitbox.transform.TransformDirection(Vector3.forward), defaults.MEGAPUNCH_DATA);

		LaunchPunch(mega_punch, defaults.MEGAPUNCH_STAMINA);
	}

	public void LaunchPunch(Punch punch, float stamina_used) {
		if (fovKick) { fovKick.SmallKick(); }
		if (screenShake) { screenShake.Shake(0.05f); }

		PunchRecord record = ExecutePunch(punch, stamina_used);

		AssessPunchRecord(record);
	}

	/** returns true if we hit something */
	PunchRecord ExecutePunch(Punch punch, float stamina_used) {

		PunchRecord record = new PunchRecord();
		Collider[] punched = Physics.OverlapBox(punch_hitbox.transform.position, punch_hitbox.transform.localScale/2, punch_hitbox.transform.rotation, punchables_mask);		

		SpendStamina(stamina_used);

		List<int> punched_ids = new List<int>();

		Vector3 look_dir = punch_hitbox.transform.TransformDirection(Vector3.forward);
		float punch_width = punch_hitbox.transform.localScale.x;
		float punch_height = punch_hitbox.transform.localScale.y;
		float punch_length = punch_hitbox.transform.localScale.z;
		// Center of flat plane perpendicular to ghost puncher at base of punch hitbox
		Vector3 look_start = punch_hitbox.transform.position - look_dir * punch_length/2;


		Vector3 look_top = look_start + punch_hitbox.transform.TransformDirection(Vector3.up) * punch_height/2;
		Vector3 look_bottom = look_start - punch_hitbox.transform.TransformDirection(Vector3.up) * punch_height/2;
		Vector3 look_left = look_start + punch_hitbox.transform.TransformDirection(Vector3.left) * punch_width/2;
		Vector3 look_right = look_start - punch_hitbox.transform.TransformDirection(Vector3.left) * punch_width/2;


		//float hitbox_angle = Mathf.Atan(punch_width / punch_length);
		//float hitbox_angle = 0.1366432f;
		//float cross_beam = Mathf.Sqrt(punch_width * punch_width + punch_length * punch_length);
		//float cross_beam = 1.165054f;
		//Debug.Log(hitbox_angle+", "+cross_beam);

		//Debug.DrawRay(look_top, look_dir*punch_length, Color.red, 1.0f);
		//Debug.DrawRay(look_bottom, look_dir*punch_length, Color.blue, 1.0f);
		//Debug.DrawRay(look_left, look_dir*punch_length, Color.green, 1.0f);
		//Debug.DrawRay(look_right, look_dir*punch_length, Color.yellow, 1.0f);

		// cast a ray from look dir toward target
		// TODO: raycast set up at the moment means you need to be looking right at the thing you're hitting. I honestly don't think anyone will notice lol
		RaycastHit[] hits = Physics.RaycastAll(new Ray(look_start, look_dir), punch_length, punchables_mask);

		foreach (Collider col in punched) {
			RaycastHit? relevant_hit = null;
			foreach (RaycastHit hit in hits) {
				if (hit.collider.gameObject == col.gameObject) {
					relevant_hit = hit;
					break;
				}
			}

			//Punch punch_copy = punch; // copy?
			
			ProcessPunchTarget(col.gameObject, punch, punched_ids, ref record, relevant_hit);
		}

		return record;

	}

	/** Iterate through raycast all hits and find the hit point */
	void DetermineHitPoint(GameObject target, ref RaycastHit[] hits) {
	}


	void ProcessPunchTarget(GameObject target, Punch punch, List<int> punched_ids, ref PunchRecord record, RaycastHit? relevant_hit) {

		// May want to move this up later. Also, do we need to cast a ray to get the hit point for particles ??
		if (relevant_hit is not null && punch.hit_class-1 < punch_particles.Count && punch_particles[punch.hit_class-1]) {
			Instantiate(punch_particles[punch.hit_class-1], relevant_hit.Value.point, this.transform.rotation);
		}

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
			ghost.GetPunched(punch, relevant_hit);
			punched_ids.Add(ghost_id);

			record.hit_ghost = true;
		}
	}

	// After a punch is executed, assess the record to see what bonuses we get
	void AssessPunchRecord(PunchRecord record) {
		if (record.items_hit > 0) {
			stamina += defaults.STAMINA_GAINED_ON_HIT;
			ti_fear_drain_pause.Reset();
		}

		if (record.hit_ghost) {
			this.fear_meter += defaults.PUNCH_FEAR;
			ti_fear_drain_pause.Reset();
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
		ti_fear_drain_pause.Tick(Time.deltaTime);
		if (ti_fear_drain_pause.Finished()) {
			float fear_drain_amount = fear_drain * GetFearRequired();
			this.fear_meter -= fear_drain_amount * Time.deltaTime;
		}

		if (this.fear_meter < 0) { this.fear_meter = 0; }

		if (this.fear_meter <= 0) {
			ti_fear_last_chance.Tick(Time.deltaTime);
			if (ti_fear_last_chance.Finished()) { ResetFear(); }
		}

		if (this.fear_meter >= GetFearRequired() && this.fear_index < this.max_fear_index) {
			this.fear_index += 1;
			this.fear_meter = GetFearRequired() * 0.2f;
		}

		if (this.fear_meter > GetFearRequired()) { this.fear_meter = GetFearRequired(); }

	}

	
	/** Get's the fear required for the next fear tier. */
	public float GetFearRequired() {
		if (this.fear_index < this.max_fear_index) {
			return defaults.FEAR_REQUIRED[this.fear_index+1];
		}
		return defaults.FEAR_REQUIRED[this.fear_index];
	}
 	
	/** Gets the fear mutlipler (applied to damage) */
	public float GetFearMultiplier() {
		return defaults.FEAR_MULTIPLIERS[this.fear_index];
	}

	/** Reset fear to 0 */
	public void ResetFear() {
		this.fear_multiplier = 1;
		this.fear_meter = 0;
		this.fear_index = 0;
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
		this.enabled = true;
		GetComponentInChildren<CameraController>().enabled = true;
	}

	public void EndRun() {
		GoDormant();
	}

/** NOTE: This only handles the state of ghost puncher - there's a lot more going on in terms of handling the end of a run, but that's handled mostly in ShopMaster */
	void GoDormant() {
		GetComponentInChildren<CameraController>().enabled = false;
		//ChangeAnimation("KickedOutEnd");
		this.enabled = false;
		
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


