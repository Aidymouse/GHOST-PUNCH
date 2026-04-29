using UnityEngine;

public enum ObjectWeight {
	CUSTOM,
	LIGHT,
	MEDIUM,
	HEAVY,
	VERY_HEAVY,
}

public class BreakableObject : MonoBehaviour
{

	public GameObject broken_obj;
	public float hp;
	/* When the broken object spawns, apply this rotation. */
	public Vector3 rotation_offset;
	// TODO: damage reduction modifier ?

	[Tooltip("If you set weight to anything other than custom, hit attributes will be informed by the ObjectAttributes scriptable object")]
	public ObjectWeight weight;
	public ObjectAttributes attrs;

	[Header("Particles")]
	[Tooltip("Particles spawned at hit location when hit")]
	public ParticleSystem hit_particles;
	[Tooltip("Particles spawned for each child object when breaking into pieces")]
	public ParticleSystem break_particles;
	[Tooltip("Particles this object spawns for itself when it breaks")]
	public ParticleSystem break_self_particles;

	[Header("Custom Hit Attributes")]
	[Tooltip("How much poise damage to deal when hitting the ghost")]
	public float poise_damage = 0;
	[Tooltip("How much HP damage to deal when hitting the ghost")]
	public float ghost_damage = 0;
	[Tooltip("How much object damage to deal when smashing into another object")]
	public float object_damage = 0;
	[Tooltip("How much physics force to apply when hitting an object (this is in addition to normal physics engine force so usually can be 0)")]
	public float force = 0;
	[Tooltip("(3) heavy object; (4) light object")]
	public int hit_class = 4;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (weight == ObjectWeight.LIGHT) {
			poise_damage = attrs.LIGHT_POISE_DAMAGE;
			object_damage = attrs.LIGHT_OBJECT_DAMAGE;
			ghost_damage = attrs.LIGHT_GHOST_DAMAGE;
			force = attrs.LIGHT_FORCE;
		} else if (weight == ObjectWeight.MEDIUM) {
			poise_damage = attrs.MEDIUM_POISE_DAMAGE;
			object_damage = attrs.MEDIUM_OBJECT_DAMAGE;
			ghost_damage = attrs.MEDIUM_GHOST_DAMAGE;
			force = attrs.MEDIUM_FORCE;
		} else if (weight == ObjectWeight.HEAVY) {
			poise_damage = attrs.HEAVY_POISE_DAMAGE;
			object_damage = attrs.HEAVY_OBJECT_DAMAGE;
			ghost_damage = attrs.HEAVY_GHOST_DAMAGE;
			force = attrs.HEAVY_FORCE;
		} else if (weight == ObjectWeight.VERY_HEAVY) {
			poise_damage = attrs.VERY_HEAVY_POISE_DAMAGE;
			object_damage = attrs.VERY_HEAVY_OBJECT_DAMAGE;
			ghost_damage = attrs.VERY_HEAVY_GHOST_DAMAGE;
			force = attrs.VERY_HEAVY_FORCE;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnCollisionEnter(Collision col) {

		// We don't need to worry about dealing damage from incoming objects because their breakable object scripts will take care of it
		if (col.gameObject.tag == "BreakableObject") {
			if (col.relativeVelocity.magnitude > 6) {
				// Punch dat freaking object yo!
				BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
				if (bo) {
					// TODO: make damage based on relative velocity ?
					Vector3 toCollided = this.transform.position - col.gameObject.transform.position;
					Punch objectPunch = new Punch(toCollided.normalized, force, object_damage, ghost_damage, poise_damage, hit_class, 0);
					bo.GetPunched(objectPunch, col.contacts[0].point);
				}
			}
		} else if (col.gameObject.tag == "GhostBodyCollider") {
			if (col.relativeVelocity.magnitude > 6) {
				Ghost ghost = col.gameObject.GetComponentInParent<Ghost>();
				if (ghost) {
					Debug.Log("Object collision with '" + col.gameObject.tag + "' at " + col.relativeVelocity.magnitude);
					Punch objectPunch = new Punch(GetComponent<Rigidbody>().linearVelocity.normalized, force, object_damage, ghost_damage, poise_damage, hit_class, 0);
					ghost.GetPunched(objectPunch);

				} 			

				// TODO: I should probably also take some damage - a ghost just flew into me!
				// This probably will end up being a punch. Oh well.
			}

		} else {
			if (col.relativeVelocity.magnitude > 6) {
					// This thing won't be damaging me, so I should take damage.
					TakeDamage(1000);
			}
		}
		
	}

	public void GetPunched(Punch punch) {
		GetPunched(punch, transform.position);
	}

	/** Apply force, then deal damage. Force should be conserved in Break logic */
	public void GetPunched(Punch punch, Vector3 hit_point) {

			// spawn particles
			// TODO: rotation
			if (hit_particles) {
					Instantiate(hit_particles, hit_point, new Quaternion());
			}

			Rigidbody rb = this.GetComponent<Rigidbody>();
			if (rb) {
					rb.AddForce(punch.Direction.normalized * punch.Force);
			}

			TakeDamage(punch.ObjectDamage, punch.Force, punch.Direction, hit_point);

	}

	public void TakeDamage(float damage) {
		TakeDamage(damage, 0, new Vector3(0, 0, 0), transform.position);
	}

	public void TakeDamage(float damage, float force) {
		TakeDamage(damage, force, new Vector3(0, 0, 0), transform.position);
	}

	/* @param hit_dir - If this damage was supplied by a hit, provide it here */
	public void TakeDamage(float damage, float force, Vector3 hit_dir, Vector3 hit_point) {
			// Negative HP = infinite HP
			if (hp < 0) { return; }

				hp -= damage;
				if (hp <= 0) {
					Break(force, hit_dir, hit_point);
				}
	}

	/** Spawn broken object (which may comprise of many smaller objects) and conserve the force I'm experiencing to them **/
	public void Break(float force, Vector3 hit_dir, Vector3 hit_point) {
		Transform initRotation = this.transform;
		initRotation.Rotate(this.rotation_offset); // Local space ??

		if (break_self_particles) {
				Instantiate(break_self_particles, hit_point, new Quaternion());
		}

		// Spawn broken object
		if (broken_obj) {
			GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);

			Rigidbody my_rb = this.GetComponent<Rigidbody>();
			Vector3 velocity = my_rb.linearVelocity;

			Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody crb in rbs) {
				if (break_particles) {
					Instantiate(break_particles, crb.transform.position, new Quaternion());
				}
				crb.isKinematic = false;
				crb.AddForce( (velocity+hit_dir).normalized * (velocity.magnitude + force));
			}
		}

		Destroy(this.gameObject);

	}
}
