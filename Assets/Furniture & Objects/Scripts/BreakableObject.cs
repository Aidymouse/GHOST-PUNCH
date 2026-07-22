using UnityEngine;
using UnityEditor;

public enum ObjectWeight {
	CUSTOM,
	LIGHT,
	MEDIUM,
	HEAVY,
	VERY_HEAVY,
}

public class BreakableObject : MonoBehaviour
{

	[Header("Attributes")]
	[Tooltip("Amount of HP this object has")]
	public float hp;
	[Tooltip("If you set weight to anything other than custom, hit attributes will be informed by the ObjectAttributes scriptable object")]
	public ObjectAttributes attrs;
	public ObjectWeight weight;
	[Tooltip("When I hit something, they'll use this to figure out what I am. (3) heavy object; (4) light object")]
	public int hit_class = 4;


	[Header("Broken Child")]
	public GameObject broken_obj;
	[Tooltip("Rotation offset to apply to spawned child object (on top of rotation match to parent)")]
	public Vector3 rotation_offset;
	[Tooltip("Particles spawned for each child object when breaking into pieces")]
	public ParticleSystem break_particles;

	[Header("Particles")]
	[Tooltip("Particles spawned at hit location when hit")]
	public ParticleSystem hit_particles;
	[Tooltip("Particles this object spawns for itself when it breaks")]
	public ParticleSystem break_self_particles;

	[Header("Sound")]
	[Tooltip("The sound that plays when the object is hit (punched or hit by another object")]
	public AudioClip hitSoundEffect;
	[Tooltip("Low bound on pitch adjustment when hit sound plays")]
	public float pitchLow;
	[Tooltip("High bound on pitch adjustment when hit sound plays")]
	public float pitchHigh;
	[Tooltip("The sound that plays when this object is destroyed")]
	public AudioClip destroyedSoundEffect;

	AudioSource audio_source;

	[Header("Custom Attributes")]
	[Tooltip("How much poise damage to deal when hitting the ghost")]
	public float poise_damage = 0;
	[Tooltip("How much HP damage to deal when hitting the ghost")]
	public float ghost_damage = 0;
	[Tooltip("How much object damage to deal when smashing into another object")]
	public float object_damage = 0;
	[Tooltip("How much physics force to apply when hitting an object (this is in addition to normal physics engine force so usually can be 0)")]
	public float force = 0;

	// If we get changed to the flying object layer, we'll return to this one when we should exit it
	int? preserved_layer;
	Collider[] colliders;

	void Start()
	{
		this.preserved_layer = null;
		colliders = GetComponents<Collider>();

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

		if (hitSoundEffect) {
			audio_source = GetComponent<AudioSource>();
			if (!audio_source) {
				Debug.LogWarning("Breakable object missing an audio source component! Adding one manually...");
				this.gameObject.AddComponent<AudioSource>();
			}
			audio_source.clip = hitSoundEffect;
		}
		/*
		if(gameObject.GetComponent<AudioSource>() != null)
        {
			destroyedSound = GetComponent<AudioSource>();
			destroyedSound.clip = hitSoundEffect;
		}
		*/
		
	}

	// Update is called once per frame
	void Update()
	{

		// Min speed before we become a flying object
	  int MIN_SPEED = 6;

		// Find total height for walkthrough check
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

	  Rigidbody rb = this.GetComponent<Rigidbody>();
		if (rb && rb.linearVelocity.magnitude > MIN_SPEED) {
			if (this.gameObject.layer != LayerMask.NameToLayer("FlyingObject")) {
				this.preserved_layer = this.preserved_layer ?? this.gameObject.layer;
				this.gameObject.layer = LayerMask.NameToLayer("FlyingObject");
			}

		} else if (total_height <= attrs.WALKTHROUGH_HEIGHT) {
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
					Punch objectPunch = new Punch(toCollided.normalized, force, object_damage, ghost_damage, poise_damage, hit_class, 0);
					bo.GetPunched(objectPunch, col.contacts[0].point);
				}
			}

		// If the thing we just hit is a ghost, deal some damage to it, and also take some damage (WIP)
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
		if (audio_source) { 
			audio_source.pitch = (Random.Range(pitchLow, pitchHigh));
			audio_source.Play(); 
		}
			

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
			broken.layer = LayerMask.NameToLayer("WalkThrough");

			Rigidbody my_rb = this.GetComponent<Rigidbody>();
			Vector3 velocity = my_rb.linearVelocity;

			Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody crb in rbs) {
				if (break_particles) {
					Instantiate(break_particles, crb.transform.position, new Quaternion());
				}
				crb.isKinematic = false;
				// TODO: some actual conservation of momentum here?
				crb.AddForce( (velocity+hit_dir).normalized * (velocity.magnitude + force));
			}
		}

		if (destroyedSoundEffect) {
			SoundEmitter.Create(destroyedSoundEffect);
		}

		Destroy(this.gameObject);

	}

	public void OnCollisionEnter(Collider col) {
	}

}

