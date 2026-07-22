using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum HitClass {
	MEGA_PUNCH=0,
	LARGE_ITEM=1,
	PUNCH=2,
	ITEM=3,
}

public struct Punch {
	public Punch(Vector3 direction, float force, float object_damage, float ghost_damage, float poise_damage, int hitClass, float fear) {
		Direction = direction;
		Force = force;
		ObjectDamage = object_damage;
		GhostDamage = ghost_damage;
		PoiseDamage = poise_damage;
		HitClass = hitClass;
		Fear = fear;
	}
	public Vector3 Direction;
	public float Force;
	public float ObjectDamage;
	public float PoiseDamage;
	public float GhostDamage;
	// Only used by the ghost
	public float Fear;
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

	CharacterController controller;
	float move_speed;

	public PuncherDefaults defaults; 
	public GhostPowerAttribs power_attribs;

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

	public AudioSource footstepSound;
	public AudioClip footSound1;
	public AudioClip footSound2;
	public float pitchLow;
	public float pitchHigh;
	public float stepCooldown;
	private float stepRate;
	private bool isMoving;

	/* Other */
	int ectoplasm = 0;

	public LayerMask punchables_mask;
	public BoxCollider punch_hitbox;


	public Animator arm_animator;

	/* Camera effects */
	FOVKick fovKick;
	ScreenShake screenShake;

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

	void Start()
	{
		action_chargePunch = InputSystem.actions.FindAction("ChargePunch");
		action_attack = InputSystem.actions.FindAction("Attack");
		action_move = InputSystem.actions.FindAction("Move");

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
		ti_charge_up.deactivate();
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

	}



	// Update is called once per frame
	void Update()
	{
		if (inCutscene)	{
			//controller.Move(Vector3.zero);
			return;
		}

		/*
		Vector3 look_dir = punch_hitbox.transform.TransformDirection(Vector3.forward);
		Vector3 look_start = punch_hitbox.transform.position - look_dir * punch_hitbox.transform.localScale.z/2;
		*/

		// Timers
		this.tick_timers();

		// Attacking
		if (ti_punch_again.finished_this_frame()) {
			punch_with = "Right";
		}

		if ((action_chargePunch.WasPerformedThisFrame() && !buffered_punch) || (buffered_charge && ti_punch_cooldown.finished_this_frame())) {

			buffered_charge = true;

			if (ti_punch_cooldown.finished()) {
				ti_charge_up.activate();
				ti_charge_up.reset();
				ChangeAnimation("ARM_CHARGE_WINDUP");
				charging_punch = true;
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
					punch_with = punch_with == "Right" ? "Left" : "Right";
				} 


				if (ti_charge_up.finished() && stamina > 0) {
					// TODO: feebler animation if this happens
					DoMegaPunch();
					ti_punch_cooldown.set(GetMegaPunchCooldown());	
				} else {
					DoPunch();
					ti_punch_cooldown.set(GetPunchCooldown());	
					ti_punch_again.reset();	
				}

				charging_punch = false;

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
				isMoving = true;
			}
		} else if (arm_animator.GetBool("Walking")) {
			StopAnimation(1);
			arm_animator.SetBool("Walking", false);
			isMoving = false;
		}

		


		/* Push */
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

		/* Stamina */
		if (ti_stamina_recharge.finished() && !charging_punch) {
			stamina += stamina_recharge_rate * Time.deltaTime;
			if (stamina > max_stamina) { stamina = max_stamina; }
		}

		/* Execute the move */
		controller.Move(move_vec * Time.deltaTime);

		//controller.move(move_vec);

		//Footsteps
		if (isMoving == true && stepCooldown < 0f)
		{
			if (footstepSound.clip = footSound1)
			{
				footstepSound.clip = footSound2;
			}
			if (footstepSound.clip = footSound2)
			{
				footstepSound.clip = footSound1;
			}
			footstepSound.pitch = (Random.Range(pitchLow, pitchHigh));
			footstepSound.Play();
			stepCooldown = stepRate;
		}
		stepCooldown -= Time.deltaTime;
		

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
			2,
			defaults.PUNCH_FEAR
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
			1,
			defaults.MEGAPUNCH_FEAR
		);

		ExecutePunch(mega_punch, defaults.MEGAPUNCH_STAMINA);
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
		ti_stamina_recharge.tick(Time.deltaTime);


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
	public void SpendStamina(float stamina_used) {
		if (stamina_used == 0) { return; }
		ti_stamina_recharge.reset();
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
		arm_animator.gameObject.SetActive(false);
		inCutscene = true;

	}


	public void ApplyItems(ItemRecord record) {
		for (int i=0; i<record.items.Count; i++) {
			Item item = record.items[i];
			item.ApplyToGhostPuncher(this);
		}
	}



	/** **/
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


