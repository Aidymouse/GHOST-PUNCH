using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Unity.Properties;
using UnityEngine.InputSystem;

using UnityEditor;

public class Ghost : MonoBehaviour
{

	public GhostDefaults defaults;
	public GhostPowerAttribs power_attribs;

	// Forces are applied to the rig core to send the ghost flying
	public Rigidbody rig_core;

	public float escape_meter;
	public float escape_needed;

	// Hit points
	[HideInInspector]
	public float hp;

	public GameObject ghostPuncher_obj;
	[HideInInspector]
	public GhostPuncher ghostPuncher;

	/** Used to find colliders and rigidbodies for switching between ragdoll and animator */
	public GameObject rig;

	[HideInInspector]
	public GameObject nav_destination;
	ParticleSystem charge_particles;
	Animator anim;

	public enum GhostActions {
		CHARGING_ESCAPE,
		STARTLED,
		MOVING_ROOM,
		HIT_STUN,
		RAGDOLL,
		RECOVERY,

		USING_POWER,
	};

	[Header("Sound Effects")]
	public AudioSource currentSound;
	public AudioClip takingDamageSound;
	public AudioClip ragdollSound;
	//Sounds temporarily stored on objects lol
	public AudioClip energySound;
	public AudioClip jumpscareSound;
	public float pitchLow;
	public float pitchHigh;



	public GhostActions cur_action;
	public GhostAction[] actions;

	// jumpscare sequence
	public bool jumpscareReady = false;
	public float jumpscareDistance = 2.0f;
	public Transform jumpscareAlignPoint;
	public Transform jumpscareTarget;
	public PlayableDirector jumpscareTimeline;
	public bool inJumpscare = false;

	public ParticleSystem ectoplasm_particles;

	public float fear_meter;

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
	[HideInInspector]
	public float poise;
	// If the ghost is vulnerable, a mega punch will send her flying
	bool vulnerable;

	Rigidbody[] rig_rbs;
	Collider[] rig_colliders;
	CharacterJoint[] rig_joints;

	float turn_speed;

	void Start()
	{

		ghostPuncher = ghostPuncher_obj.GetComponent<GhostPuncher>();

		/* Init Actions */
		actions = new GhostAction[7];
		actions[(int)GhostActions.CHARGING_ESCAPE] = new GhostAction_ChargingEscape(this);
		actions[(int)GhostActions.MOVING_ROOM] = new GhostAction_MovingRoom(this);
		

		rig_rbs = rig.GetComponentsInChildren<Rigidbody>();
		rig_colliders = rig.GetComponentsInChildren<Collider>();
		rig_joints = rig.GetComponentsInChildren<CharacterJoint>();

		DisableRagdoll();

		poise = defaults.POISE;
		// needs to update on ghost slap somehow too		

		turn_speed = defaults.TURN_SPEED;
		fear_meter = 0;

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

		/* Powers */
		// Set up last so any objects retrieved in constructors are present
		powers = new GhostPower[3];
		powers[0] = new GhostPower_Wave(this, power_attribs);
		powers[1] = new GhostPower_Slap(this, power_attribs);
		powers[2] = new GhostPower_Scream(this, power_attribs);

		EnterAction(GhostActions.MOVING_ROOM);

		currentSound = GetComponent<AudioSource>();
		currentSound.clip = takingDamageSound;

	}

	// Update is called once per frame
	public void Update()
	{
		if (inJumpscare)
		{
		    return; // freeze everything during jumpscare, she should still be punchable so idk we'll need to come back to this
		}
		
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
		Vector3 to_target = 
				nav_agent.steeringTarget 
				- transform.position;
		if (to_target.magnitude > 0) {
			to_target.y = 0;
			Quaternion target_angle = Quaternion.LookRotation(to_target);
			nav_agent.transform.rotation = target_angle;
		}
		//transform.TurnTowards(ghostPuncher.transform);

		if (ti_restore_poise.finished_this_frame()) {
			RestorePoise();
		}

		/* Actions */
		switch (cur_action) {
			case GhostActions.CHARGING_ESCAPE: 
				state_ChargingEscape();
				break;

			case GhostActions.MOVING_ROOM: 
				state_MovingRoom();
				break;

			case GhostActions.HIT_STUN: 
				state_HitStun();
				break;

			case GhostActions.RAGDOLL: 
				state_Ragdoll();
				break;

			case GhostActions.RECOVERY: 
				state_Recovery();
				break;

			case GhostActions.USING_POWER: 
				//state_HitStun();
				state_UsingPower();
				break;


		}

		CheckJumpscareTrigger();

	    // manual jumpscare trigger
		if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
		{
		    TriggerJumpscare();
		}

		tick_timers();

	}

	/* Jumpscare trigger check */
	void CheckJumpscareTrigger()
	{
		if (!jumpscareReady) return;
			float dist = Vector3.Distance(transform.position, ghostPuncher.transform.position);

		if (dist <= jumpscareDistance)
		{
   			TriggerJumpscare();
		}
	}

	void ExitAction() {
		// Logic based on what state we're leaving
		switch (cur_action) {
			case GhostActions.CHARGING_ESCAPE: 
				charge_particles.Stop();
				break;

			case GhostActions.RAGDOLL:

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

	public void EnterAction(GhostActions action) {

		ExitAction();


		// Enter New State Logic
		switch (action) {
			case GhostActions.CHARGING_ESCAPE: 
				// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
				charge_particles.Play();
				cur_action = action;
				ChangeAnimation("ChargeEscape");
				break;


			case GhostActions.MOVING_ROOM: 
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


			case GhostActions.HIT_STUN: 

				ti_hit_stun.reset();

				PlayAnimation("Hit_Cower");
				nav_agent.isStopped = true;
				cur_action = action;
				break;

			case GhostActions.RAGDOLL:
				cur_action = action;
				ti_ragdoll.reset();
				DisableAnimator();
				EnableRagdoll();
				nav_agent.isStopped = true;
				vulnerable = false;
				break;

			case GhostActions.RECOVERY:
				ti_recovery.activate();
				// TODO: if the ghost was attacking this should probably be 0...
				ChangeAnimation("Idle", ti_recovery.time_remaining);
				cur_action = action;
				break;


			case GhostActions.USING_POWER: 
				cur_action = action;
				PickRandomPower();
				break;

		}

	}





	/** STATES **/

	void state_MovingRoom() {
		actions[(int)GhostActions.MOVING_ROOM].Update();
	}

	void state_ChargingEscape() {
		// TODO: be making wubwubwubwubwubwubwub sound

		actions[(int)GhostActions.CHARGING_ESCAPE].Update();

		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}

	void state_HitStun() {
		ti_hit_stun.tick(Time.deltaTime);

		if (ti_hit_stun.finished_this_frame()) {
			ti_recovery.set(0);
			EnterAction(GhostActions.RECOVERY);
		}
	}

	void state_Ragdoll() {
		ti_ragdoll.tick(Time.deltaTime);

		if (ti_ragdoll.finished_this_frame()) {
			ti_recovery.set(1);
			EnterAction(GhostActions.RECOVERY);
		}

	}

	void state_Recovery() {
		ti_recovery.tick(Time.deltaTime);

		if (ti_recovery.finished()) {
			RestorePoise();
			nav_agent.isStopped = false;

			EnterAction(GhostActions.USING_POWER);
		/*
			if (nav_destination == null) {
			} else {
				EnterAction(GhostActions.MOVING_ROOM);
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
			EnterAction(GhostActions.MOVING_ROOM);
		}
	}

	void tick_timers() {
		if (cur_action != GhostActions.HIT_STUN) {
			ti_restore_poise.tick(Time.deltaTime);
		}
	}

	/* Jumpscare sequence */
	public void TriggerJumpscare()
	{
	    if (inJumpscare) return;
	    inJumpscare = true;
			Debug.Log("JUMPSCARED");

	    // Freeze AI
        nav_agent.isStopped = true;

	    // align point to target, this doesn't fucking work.
       Vector3 offset = transform.position - jumpscareAlignPoint.position;
       transform.position = jumpscareTarget.position + offset;
       transform.rotation = jumpscareTarget.rotation;

	    // Lock player movement
       ghostPuncher.inCutscene = true;

	    // Play timeline
		jumpscareTimeline.time = 0;
		jumpscareTimeline.Play();
	}

	public void EndJumpscare()
	{
	    inJumpscare = false;
      nav_agent.enabled = true;
      nav_agent.updatePosition = true;
      nav_agent.updateRotation = true;
      anim.enabled = true;
      ghostPuncher.GetComponent<GhostPuncher>().inCutscene = false;
	}

	/** EVENTS **/
	public void GetPunched(Punch punch) {

		hp -= punch.GhostDamage;
		fear_meter += punch.Fear;

		currentSound.clip = takingDamageSound;
		currentSound.pitch = (Random.Range(pitchLow, pitchHigh));
		currentSound.Play();


		// 1 is mega punch and 3 is big object hit
		if (vulnerable && (punch.HitClass == 1 || punch.HitClass == 3)) {
			
			Ragdoll(punch);
			return;
		}
		

		if (HasHyperArmor()) {
			return;
		}
	
		poise -= punch.PoiseDamage;
		
		if (ectoplasm_particles) {
			Instantiate(ectoplasm_particles, transform.position, new Quaternion());
		}

		if (poise <= 0) {
			if (punch.HitClass <= 1) {
				Ragdoll(punch);
				
			} else {
				BecomeVulnerable();
				EnterAction(GhostActions.HIT_STUN);
			}
		} else {
			ti_restore_poise.reset();

			// TODO: play a random hit animation
			//int hurt_num = Random.Range(1,3);
			//PlayAnimation("Hurt"+hurt_num);
			PlayAnimation("Hurt1");

			if (cur_action == GhostActions.CHARGING_ESCAPE) {
				// TODO: there should be a bit of buffer time here or powers come out super fast
				EnterAction(GhostActions.USING_POWER);
			}
		}
	}

	void GainFear(int fear_gained) {
		fear_meter += fear_gained;
	}

	void LoseFear(int fear_lost) {
		fear_meter -= fear_lost;
	}

	void RestorePoise() {
		poise = defaults.POISE;
	}

	void BecomeVulnerable() {
		vulnerable = true;
		fear_meter += 5;
	}

	void StopBeingVulnerable() {
		vulnerable = false;
	}

	void Ragdoll(Punch punch) {
		currentSound.clip = ragdollSound;
		currentSound.PlayOneShot(ragdollSound);
		currentSound.Play();
		EnterAction(GhostActions.RAGDOLL);
		rig_core.AddForce(punch.Direction * punch.Force * defaults.MAKE_HER_FLY_FACTOR);
	}

	/** STATUS **/
	// If the ghost has hyper armor, she cannot have her poise break (it can go down though)
	bool HasHyperArmor() {
		return cur_action == GhostActions.HIT_STUN || cur_action == GhostActions.RAGDOLL || cur_action == GhostActions.RECOVERY;

	}

	bool SpinDisabled() {
		return cur_action == GhostActions.RAGDOLL;
	}

	public bool Escaped() {
		return escape_meter >= escape_needed;
	}

	/** RAGDOLL **/

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
