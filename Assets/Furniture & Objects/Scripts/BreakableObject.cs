using UnityEngine;
using UnityEditor;

public class BreakableObject : MonoBehaviour
{

	[Header("Attributes")]
	[Tooltip("Amount of HP this object has")]
	public float hp;
	[Tooltip("Controls various misc object configs. Probably standard across all objects.")]
	public ObjectConfig config;
	[Tooltip("Attributes contains damage and force attributes for an object")]
	public ObjectAttributes attrs;
	[Tooltip("Material controls things like particles and sound effects")]
	public ObjectMaterial material;
	[Tooltip("When I hit something, they'll use this to figure out what I am. (3) heavy object; (4) light object")]
	public int hit_class = 4;


	[Header("Broken Child")]
	public GameObject broken_obj;
	[Tooltip("Rotation offset to apply to spawned child object (on top of rotation match to parent)")]
	public Vector3 rotation_offset;
	
	[HideInInspector]
	public float poise_damage;
	[HideInInspector]
	public float ghost_damage;
	[HideInInspector]
	public float object_damage;
	[HideInInspector]
	public float force;

	AudioSource audio_source;

	[Header("Old stuff")]
	public ParticleSystem hit_particles;
	public ParticleSystem break_particles;
	public ParticleSystem break_self_particles;
	public float pitchLow;
	public float pitchHigh;
	public AudioClip hitSoundEffect;
	public AudioClip destroyedSoundEffect;

	// If we get changed to the flying object layer, we'll return to this one when we should exit it
	int? preserved_layer;
	Collider[] colliders;

	void Start()
	{

		if (!this.attrs) {
			Debug.LogError("No object attrs");
		}
		
		if (!this.config) {
			Debug.LogError("No object config");
		}

		if (!this.material) {
			Debug.LogWarning("BreakableObject has no material selected");
		}

		this.preserved_layer = null;
		colliders = GetComponents<Collider>();

		poise_damage = attrs.POISE_DAMAGE;
		object_damage = attrs.OBJECT_DAMAGE;
		ghost_damage = attrs.GHOST_DAMAGE;
		force = attrs.FORCE;

		if (material && material.hit_sound) {
			audio_source = GetComponent<AudioSource>();
			if (!audio_source) {
				Debug.LogError("Breakable object missing an audio source component! Adding one manually...");
				audio_source = this.gameObject.AddComponent<AudioSource>();
			}
			audio_source.clip = material.hit_sound;
		}
		
	}

	// Update is called once per frame
	void Update()
	{

		// Min speed before we become a flying object
	  int MIN_SPEED = 6;

		// Find total height for walkthrough check
		float total_height = GetBoundingBoxHeight();

	  Rigidbody rb = this.GetComponent<Rigidbody>();
		if (rb && rb.linearVelocity.magnitude > MIN_SPEED) {
			if (this.gameObject.layer != LayerMask.NameToLayer("FlyingObject")) {
				this.preserved_layer = this.preserved_layer ?? this.gameObject.layer;
				this.gameObject.layer = LayerMask.NameToLayer("FlyingObject");
			}

		} else if (total_height <= config.WALKTHROUGH_HEIGHT) {
			if (this.gameObject.layer != LayerMask.NameToLayer("WalkThrough")) {
				this.preserved_layer = this.preserved_layer ?? this.gameObject.layer;
				this.gameObject.layer = LayerMask.NameToLayer("WalkThrough");
			}

		} else if (this.preserved_layer != null) {
			this.gameObject.layer = this.preserved_layer.Value;
			this.preserved_layer = null;
		}
		
	}


	public void OnCollisionEnter(Collision col) {

		// TODO: on touch floor, become normal grounded object. Might not be needed

		// If the thing we are colliding with is a breakable object, deal some damage!
		// We don't need to worry about taking damage from incoming objects because their breakable object scripts will take care of it
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

		// If the thing we just hit is a ghost, deal some damage to it, and also take some damage (WIP)
		} else if (col.gameObject.tag == "GhostBodyCollider") {
			if (col.relativeVelocity.magnitude > 6) {
				Ghost ghost = col.gameObject.GetComponentInParent<Ghost>();
				if (ghost) {
					Debug.Log("Object collision with '" + col.gameObject.tag + "' at " + col.relativeVelocity.magnitude);
					Punch objectPunch = new Punch(GetComponent<Rigidbody>().linearVelocity.normalized, force, object_damage, ghost_damage, poise_damage, hit_class);
					ghost.GetPunched(objectPunch);
				} 			

				// TODO: I should probably also take some damage - a ghost just flew into me!
				// This probably will end up being a punch. Oh well.
			}

		} else {
			if (col.relativeVelocity.magnitude > 6) {
					// This thing won't handle damaging me, so I should take damage.
					TakeDamage(200);
			}
		}
		
	}

	public void GetPunched(Punch punch) {
		
		GetPunched(punch, transform.position);

	}

	/** Apply force, then deal damage. Force should be conserved in Break logic */
	public void GetPunched(Punch punch, Vector3 hit_point) {


		//Audio
		// A breakable object only makes one sound, when it's hit, so we don't need to assign the sound, just play it.
		if (audio_source) { 
			audio_source.pitch = (Random.Range(material.pitch_low, material.pitch_high));
			audio_source.Play(); 
		}
			

		// spawn particles
		// TODO: rotation
		if (material && material.hit_particles) {
					Instantiate(material.hit_particles, hit_point, new Quaternion());
			}

			Rigidbody rb = this.GetComponent<Rigidbody>();
			if (rb) {
					rb.AddForce(punch.direction.normalized * punch.force);
			}

			TakeDamage(punch.object_damage, punch.force, punch.direction, hit_point);

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

		if (material && material.break_particles) {
				Instantiate(material.break_particles, hit_point, new Quaternion());
		}

		// Spawn broken object
		if (broken_obj) {
			GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);
			broken.layer = LayerMask.NameToLayer("WalkThrough");

			Rigidbody my_rb = this.GetComponent<Rigidbody>();
			Vector3 velocity = my_rb.linearVelocity;

			Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody crb in rbs) {
				// TODO: use child breakable object material spawn particles
				/*
				if (material.spawn_) {
					Instantiate(break_particles, crb.transform.position, new Quaternion());
				}
				*/

				crb.isKinematic = false;
				// TODO: some actual conservation of momentum here?
				crb.AddForce( (velocity+hit_dir).normalized * (velocity.magnitude + force));
			}
		}

		if (material && material.break_sound) {
			SoundEmitter.Create(material.break_sound);
		}

		Destroy(this.gameObject);

	}

	public void OnCollisionEnter(Collider col) {
	}

	public float GetBoundingBoxHeight() {
		float total_height = 100;
		
		if (colliders.Length > 0) {
			float lowest_min = 99999;
			float highest_max = -99999;

			foreach (Collider c in colliders) {
				float min = c.bounds.min.y;
				if (min < lowest_min) { lowest_min = min; }
				float max = c.bounds.max.y;
				if (max > highest_max) { highest_max = max; }
			}

			total_height = highest_max - lowest_min;
		}

		return total_height;
	}

}

