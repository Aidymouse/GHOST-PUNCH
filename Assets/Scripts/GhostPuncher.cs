using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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



public class GhostPuncher : MonoBehaviour
{
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
  string punch_with = "RIGHT";
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

  LayerMask layer_punchable;

  public Vector3 lose_point;

  Animator arm_animator;

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

    arm_animator = this.GetComponentInChildren<Animator>();
    layer_punchable = LayerMask.GetMask("Punchable");

    /* Load Defaults */
    move_speed = defaults.MOVE_SPEED;
    max_stamina = defaults.MAX_STAMINA;
    stamina = defaults.MAX_STAMINA;
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
      }
    } else if (arm_animator.GetBool("Walking")) {
      StopAnimation(1);
      arm_animator.SetBool("Walking", false);
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

		


  }

  void DoPunch() {
    ChangeAnimation("PUNCH_"+punch_with);

    fovKick.SmallKick();
    screenShake.Shake(0.05f);

    ExecutePunch(defaults.PUNCH_FORCE, defaults.PUNCH_OBJECT_DAMAGE, defaults.PUNCH_GHOST_DAMAGE, defaults.PUNCH_POISE_DAMAGE, 2, defaults.PUNCH_STAMINA, defaults.PUNCH_FEAR);
  }

  void DoMegaPunch() {
    ChangeAnimation("CHARGE_PUNCH");

    fovKick.BigKick();
    screenShake.Shake(0.2f);

    ExecutePunch(defaults.MEGAPUNCH_FORCE, defaults.MEGAPUNCH_OBJECT_DAMAGE, defaults.MEGAPUNCH_GHOST_DAMAGE, defaults.MEGAPUNCH_POISE_DAMAGE, 1, defaults.MEGAPUNCH_STAMINA, defaults.MEGAPUNCH_FEAR);
  }

  void ExecutePunch(float force, float object_damage, float ghost_damage, float poise_damage, int hitClass, float stamina_used, float fear) {

    SpendStamina(stamina_used);

    // Cast a ray - jeff says should be a box
    RaycastHit attack_hit;

    Camera cam = this.GetComponentInChildren<Camera>();

    //Vector3 ray_dir = transform.TransformDirection(Vector3.forward);
    Vector3 ray_dir = cam.transform.TransformDirection(Vector3.forward);

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
		

    if (Physics.Raycast(cam.transform.position, ray_dir, out attack_hit, punch_range, layer_punchable)) {
      Debug.DrawRay(transform.position, ray_dir, Color.red, 1, false);

      Collider hit_col = attack_hit.collider;

      if (hit_col == null) {
	return;
      }

      Punch punch = new Punch(ray_dir, force, object_damage, ghost_damage, poise_damage, hitClass, fear);

      // Spawn Punch Particles
      if (hitClass-1 < punch_particles.Count && punch_particles[hitClass-1]) {
	Instantiate(punch_particles[hitClass-1], attack_hit.point, this.transform.rotation);
      }

      // Receiver handle punch
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

  public void EndRun() {
    this.transform.position = lose_point;

  }

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


