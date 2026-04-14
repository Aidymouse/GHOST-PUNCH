using UnityEngine;


public class BreakableObject : MonoBehaviour
{

	public GameObject broken_obj;
	public float hp;
	/* When the broken object spawns, apply this rotation. */
	public Vector3 rotation_offset;
	// TODO: damage reduction modifier ?
	[Tooltip("Particles spawned at hit location when hit")]
	public ParticleSystem hit_particles;
	[Tooltip("Particles spawned for each child object when breaking into pieces")]
	public ParticleSystem break_particles;
	[Tooltip("Particles this object spawns for itself when it breaks")]
	public ParticleSystem break_self_particles;

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

	// TODO: hit class resistance matrix


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

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
					Punch objectPunch = new Punch(toCollided.normalized, force, object_damage, ghost_damage, poise_damage, hit_class);
					bo.GetPunched(objectPunch, col.contacts[0].point);
				}
			}
		} else if (col.gameObject.tag == "GhostBodyCollider") {
			if (col.relativeVelocity.magnitude > 6) {
				Ghost ghost = col.gameObject.GetComponentInParent<Ghost>();
				if (ghost) {
					Debug.Log("Object collision with '" + col.gameObject.tag + "' at " + col.relativeVelocity.magnitude);
					Punch objectPunch = new Punch(GetComponent<Rigidbody>().linearVelocity.normalized, force, object_damage, ghost_damage, poise_damage, hit_class);
					ghost.GetPunched(objectPunch);
				} 			

				// TODO: I should probably also take some damage - a ghost just flew into me!
			}

			// The ghost should also deal damage to me!
		} else {
			// This thing won't be damaging me, so I should take damage.
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

			TakeDamage(punch.object_damage, hit_point);

	}

	public void TakeDamage(float damage) {
		TakeDamage(damage, transform.position);
	}

	public void TakeDamage(float damage, Vector3 hit_point) {
			// Negative HP = infinite HP
			if (hp < 0) { return; }

				hp -= punch.ObjectDamage;
				if (hp <= 0) {
					Break();
				}
	}

	/** Spawn broken object (which may comprise of many smaller objects) and conserve the force I'm experiencing to them **/
	void Break() {
		Break(transform.position);
	}

	void Break(Vector3 hit_point) {
		Transform initRotation = this.transform;
		initRotation.Rotate(this.rotation_offset); // Local space ??

		if (break_self_particles) {
				Instantiate(break_self_particles, hit_point, new Quaternion());
		}

		// Spawn broken object
		if (broken_obj) {
			GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);
			//broken.SetActive(true);
			

			Rigidbody my_rb = this.GetComponent<Rigidbody>();
			Vector3 velocity = mr_rb.velocity;

			Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody crb in rbs) {
				if (break_particles) {
					Instantiate(break_particles, crb.transform.position, new Quaternion());
				}
				crb.isKinematic = false;
				crb.AddForce(velocity);
			}
		}

		Destroy(this.gameObject);

	}
}
