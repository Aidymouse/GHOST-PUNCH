using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;

using UnityEditor;

public class Ghost : MonoBehaviour
{

	public GhostDefaults defaults;
	public GhostPowerAttribs power_attribs;

	// Forces are applied to the rig core to send the ghost flying
	public Rigidbody rig_core;

	public float escape_meter;
	public float escape_needed;

	// If false, the ghost will do small staggers on hit.
	bool small_hit_resist;
	// Hit points
	float hp;

	public GameObject ghostPuncher;

	/** Used to find colliders and rigidbodies for switching between ragdoll and animator */
	public GameObject rig;

	GameObject nav_destination;
	ParticleSystem charge_particles;
	Animator anim;

	public enum GhostAction {
		CHARGING_ESCAPE,
		STARTLED,
		MOVING_ROOM,
		HIT_STUN,
		RAGDOLL,
		RECOVERY,

		USING_POWER,
	};


	public GhostAction cur_action;

	GhostPower[] powers;
	GhostPower active_power;

	// Spawns when the ghost uses her wave power
	public GameObject wave_orb;

	NavMeshAgent nav_agent;

	Timer ti_hit_stun;
	Timer ti_ragdoll;
	Timer ti_restore_poise;
	Timer ti_recovery;

	// When poise hit's 0, the ghost staggers, which makes her vulnerable.
	float poise;
	// If the ghost is vulnerable, a mega punch will send her flying
	bool vulnerable;

	Rigidbody[] rig_rbs;
	Collider[] rig_colliders;
	CharacterJoint[] rig_joints;

	float turn_speed;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

		rig_rbs = rig.GetComponentsInChildren<Rigidbody>();
		rig_colliders = rig.GetComponentsInChildren<Collider>();
		rig_joints = rig.GetComponentsInChildren<CharacterJoint>();

		DisableRagdoll();

		poise = defaults.POISE;

		turn_speed = defaults.TURN_SPEED;

		hp = defaults.HP;

		/* Timers */
		ti_hit_stun = new Timer(0, defaults.HIT_STUN_TIME);
		ti_restore_poise = new Timer(0, defaults.POISE_RESTORE_TIMER);
		ti_ragdoll = new Timer(0, defaults.RAGDOLL_TIME);
		ti_recovery = new Timer(0);

		/* Animator */
		anim = this.GetComponentInChildren<Animator>();

		/* Nav Settings */
		nav_agent = GetComponent<NavMeshAgent>();
		nav_agent.updateRotation = false;
		//nav_agent.destination = nav_destination.position;

		charge_particles = GetComponentInChildren<ParticleSystem>();

		//nav_destination = null;

		escape_meter = 0;
		escape_needed = 60;

		/* Powers */
		// Set up last so any objects retrieved in constructors are present
		powers = new GhostPower[3];
		powers[0] = new GhostPower_Wave(this, power_attribs);
		powers[1] = new GhostPower_Slap(this, power_attribs);
		powers[2] = new GhostPower_Scream(this, power_attribs);

		EnterAction(GhostAction.MOVING_ROOM);

	}

	// Update is called once per frame
	public void Update()
	{

		/* Rotate towards ghost puncher */
		// TODO: when we flee we should look that direction instead
		if (!SpinDisabled()) {
			Vector3 toGhostPuncher = ghostPuncher.transform.position - transform.position;
			toGhostPuncher.y = 0;
			Quaternion ghostPuncher_angle = Quaternion.LookRotation(toGhostPuncher);
			float turn_speed = 100;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, ghostPuncher_angle, turn_speed * Time.deltaTime);
		}


		/* Rotate nav agent always towards its next target (infinite turn speed) */
		Vector3 to_target = nav_agent.steeringTarget - transform.position;
		if (to_target.magnitude > 0) {
			to_target.y = 0;
			Quaternion target_angle = Quaternion.LookRotation(to_target);
			nav_agent.transform.rotation = target_angle;
		}
		//transform.TurnTowards(ghostPuncher.transform);

		if (ti_restore_poise.finished()) {
			RestorePoise();
		}

		/* Actions */
		switch (cur_action) {
			case GhostAction.CHARGING_ESCAPE: 
				state_ChargingEscape();
				break;

			case GhostAction.MOVING_ROOM: 
				state_MovingRoom();
				break;

			case GhostAction.HIT_STUN: 
				state_HitStun();
				break;

			case GhostAction.RAGDOLL: 
				state_Ragdoll();
				break;

			case GhostAction.RECOVERY: 
				state_Recovery();
				break;

			case GhostAction.USING_POWER: 
				//state_HitStun();
				state_UsingPower();
				break;


		}

		tick_timers();

	}

	void ExitAction() {
		// Logic based on what state we're leaving
		switch (cur_action) {
			case GhostAction.CHARGING_ESCAPE: 
				charge_particles.Stop();
				break;

			case GhostAction.RAGDOLL:

				Vector3 ragdoll_offset = transform.position - rig_core.transform.position;
				ragdoll_offset.y = 0;
				transform.position -= ragdoll_offset;
				rig_core.position += ragdoll_offset;
				//Debug.Log(ragdoll_offset);

				DisableRagdoll();
				EnableAnimator();
				break;

		};
	}

	void EnterAction(GhostAction action) {

		ExitAction();


		// Enter New State Logic
		switch (action) {
			case GhostAction.CHARGING_ESCAPE: 
				// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
				charge_particles.Play();
				cur_action = action;
				ChangeAnimation("ChargeEscape");
				break;


			case GhostAction.MOVING_ROOM: 
				if (nav_destination == null) {
					GameObject[] destinations = GameObject.FindGameObjectsWithTag("GhostDestination"); // Supposedly slow, but shouldn't be a big deal
					int dest_idx = Random.Range(0, destinations.Length);
					GameObject dest_obj = destinations[dest_idx];

					// If we've selected the area we're already at, the state fn will catch it

					nav_agent.destination = dest_obj.transform.position;
					nav_destination = dest_obj;

				} else {
					nav_agent.destination = nav_destination.transform.position;
				}

				cur_action = action;
				break;


			case GhostAction.HIT_STUN: 

				ti_hit_stun.reset();

				PlayAnimation("Hit_Cower");
				nav_agent.isStopped = true;
				cur_action = action;
				break;

			case GhostAction.RAGDOLL:
				cur_action = action;
				ti_ragdoll.reset();
				DisableAnimator();
				EnableRagdoll();
				nav_agent.isStopped = true;
				vulnerable = false;
				break;

			case GhostAction.RECOVERY:
				ti_recovery.activate();
				ChangeAnimation("Idle", ti_recovery.time_remaining);
				cur_action = action;
				break;


			case GhostAction.USING_POWER: 
				cur_action = action;
				PickRandomPower();
				break;

		}

	}





	/** STATES **/

	void state_MovingRoom() {
		if ((transform.position - nav_destination.transform.position).magnitude < 2) {
			nav_destination = null;
			EnterAction(GhostAction.CHARGING_ESCAPE);
		}
	}

	void state_ChargingEscape() {
		// TODO: be making wubwubwubwubwubwubwub sound

		float old_escape = escape_meter;
		escape_meter += Time.deltaTime;

		if (escape_meter >= escape_needed) {
			Debug.Log("You lose!");
		}
		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}

	void state_HitStun() {
		ti_hit_stun.tick(Time.deltaTime);

		if (ti_hit_stun.finished_this_frame()) {
			ti_recovery.set(0);
			EnterAction(GhostAction.RECOVERY);
		}
	}

	void state_Ragdoll() {
		ti_ragdoll.tick(Time.deltaTime);

		if (ti_ragdoll.finished_this_frame()) {
			ti_recovery.set(1);
			EnterAction(GhostAction.RECOVERY);
		}

	}

	void state_Recovery() {
		ti_recovery.tick(Time.deltaTime);

		if (ti_recovery.finished()) {
			RestorePoise();
			nav_agent.isStopped = false;

			EnterAction(GhostAction.USING_POWER);
		/*
			if (nav_destination == null) {
			} else {
				EnterAction(GhostAction.MOVING_ROOM);
			}
		*/
		}
	}

	void state_UsingPower() {
		active_power.Update();

		if (active_power.phase == GhostPower.GhostPowerPhase.DONE) {
			LeavePower();
		}
	}

	/**** POWERS ****/
	void PickRandomPower() {
		int power_index = power_attribs.OVERRIDE_POWER_IDX == -1 ? Random.Range(0, powers.Length) : power_attribs.OVERRIDE_POWER_IDX; 
		active_power = powers[power_index];
		active_power.Reset();
		active_power.Start();
	}

	void LeavePower() {
		active_power.End();

		if (Random.Range(1,4) == 3) {
			PickRandomPower();
		} else {
			EnterAction(GhostAction.MOVING_ROOM);
		}
	}

	void tick_timers() {
		ti_restore_poise.tick(Time.deltaTime);
	}

	/** EVENTS **/
	public void GetPunched(Punch punch) {

		// 1 is mega punch and 3 is big object hit
		if (vulnerable && (punch.HitClass == 1 || punch.HitClass == 3)) {
			Ragdoll(punch);
			return;
		}


		if (HasHyperArmor()) {
			return;
		}
	
		poise -= punch.PoiseDamage;

		if (poise <= 0) {
			if (punch.HitClass <= 1) {
				Ragdoll(punch);
			} else {
				BecomeVulnerable();
				EnterAction(GhostAction.HIT_STUN);
			}
		} else {
			ti_restore_poise.reset();

			// TODO: play a random hit animation
			//int hurt_num = Random.Range(1,3);
			//PlayAnimation("Hurt"+hurt_num);
			PlayAnimation("Hurt1");

			if (cur_action == GhostAction.CHARGING_ESCAPE) {
				// TODO: there should be a bit of buffer time here or powers come out super fast
				EnterAction(GhostAction.USING_POWER);
			}
		}


	}

	void RestorePoise() {
		poise = defaults.POISE;
	}

	void BecomeVulnerable() {
		vulnerable = true;
	}

	void StopBeingVulnerable() {
		vulnerable = false;
	}

	void Ragdoll(Punch punch) {
		EnterAction(GhostAction.RAGDOLL);
		rig_core.AddForce(punch.Direction * punch.Force * defaults.MAKE_HER_FLY_FACTOR);

	}

	/** STATUS **/
	// If the ghost has hyper armor, she cannot have her poise break (it can go down though)
	bool HasHyperArmor() {
		return cur_action == GhostAction.HIT_STUN || cur_action == GhostAction.RAGDOLL || cur_action == GhostAction.RECOVERY;

	}

	bool SpinDisabled() {
		return cur_action == GhostAction.RAGDOLL;
	}

	// TODO: wrap these in actual state changes so she doesn't keep trying to move around when she's ragdolled
	void EnableAnimator() {
		DisableRagdoll();
		anim.enabled = true;
	}

	void DisableAnimator() {
		anim.enabled = false;
	}

	void PlayAnimation(string new_anim) {
		//anim.Rewind(new_anim);
		anim.Play(new_anim, -1, 0.0f);
	}

	public void ChangeAnimation(string new_anim, float fade_time=0f) {
		anim.CrossFade(new_anim, fade_time);
	}

	void EnableRagdoll() {
		DisableAnimator();
		/*foreach (Collider col in rig_colliders) {
			col.enabled = true;
			}*/
		foreach (Rigidbody rb in rig_rbs) {
			rb.detectCollisions = true;
			rb.useGravity = true;
			rb.isKinematic = false;
		}
		foreach (CharacterJoint joint in rig_joints) {
			joint.enableCollision = true;
		}
	}

	void DisableRagdoll() {
		/*foreach (Collider col in rig_colliders) {
			col.enabled = false;
			}*/
		foreach (Rigidbody rb in rig_rbs) {
			//rb.detectCollisions = false;
			rb.useGravity = false;
			rb.isKinematic = true;
		}
		foreach (CharacterJoint joint in rig_joints) {
			joint.enableCollision = false;
		}
	}


	/** GETTERS */

	public NavMeshAgent get_nav_agent() { return nav_agent; }
}
