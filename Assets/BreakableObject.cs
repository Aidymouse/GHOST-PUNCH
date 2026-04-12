using UnityEngine;


public class BreakableObject : MonoBehaviour
{

	public GameObject broken_obj;
	public float hp;
	/* When the broken object spawns, apply this rotation. */
	public Vector3 rotation_offset;
	// TODO: damage reduction modifier ?
	public ParticleSystem hit_particles;
	public ParticleSystem break_particles;

	public float poise_damage = 0;
	public float ghost_damage = 0;
	public float object_damage = 0;
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
			}

			// The ghost should also deal damage to me!
		} else {
			// This thing won't be damaging me, so I should take damage.
		}
		
	}

	public void GetPunched(Punch punch) {
		GetPunched(punch, transform.position);
	}

	public void GetPunched(Punch punch, Vector3 hit_point) {

			// Roundabout logic lets us treat -1 as infinite HP
			bool broken_this_punch = false;
			if (hp > 0) {
				hp -= punch.ObjectDamage;
				if (hp <= 0) {
					broken_this_punch = true;
				}
			}

			if (broken_this_punch) {
				if (break_particles) {
					Instantiate(break_particles, hit_point, new Quaternion());
				}
				Break(punch.Force, punch.Direction);
			} else {
				// spawn particles
				// TODO: rotation
				if (hit_particles) {
					Instantiate(hit_particles, hit_point, new Quaternion());
				}

				Rigidbody rb = this.GetComponent<Rigidbody>();
				if (rb) {
					Vector3 blast_dir = punch.Direction;

					//crb.constraints = RigidbodyConstraints.None;
					rb.isKinematic = false;
					rb.AddForce(blast_dir.normalized * punch.Force);
				}
			}


	}

	void Break(float force, Vector3 punch_dir) {
		Transform initRotation = this.transform;
		initRotation.Rotate(this.rotation_offset); // Local space ??

		if (broken_obj) {
			GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);
			//broken.SetActive(true);

			Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody crb in rbs) {
				if (break_particles) {
					Instantiate(break_particles, crb.transform.position, new Quaternion());
				}
				Vector3 blast_dir = punch_dir;

				//crb.constraints = RigidbodyConstraints.None;
				crb.isKinematic = false;
				crb.AddForce(blast_dir.normalized * force);
				//crb.gameObject.layer = LayerMask.NameToLayer("Punchable");
			}
		}

		Destroy(this.gameObject);

	}
}
