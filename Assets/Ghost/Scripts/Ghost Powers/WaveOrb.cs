using UnityEngine;

public class WaveOrb : MonoBehaviour
{
	public float expansion_speed;
	public ParticleSystem wave_particles;
	public Vector3 particle_offset;
	public float life_timer;

	bool hit_puncher;

	public GAD_Blast attrs;

	public float object_damage;
	public float object_force;


	Timer life;
	void Start() {
		hit_puncher = false;

		life = new Timer(life_timer);

		Instantiate(wave_particles, transform.position + particle_offset, transform.rotation);
	}

	void Update() {

		float expansion = expansion_speed * Time.deltaTime;

		transform.localScale += new Vector3(expansion, expansion, expansion);

		life.Tick(Time.deltaTime);
		if (life.Finished()) {
			Destroy(this.gameObject);
		}


	}


	void OnTriggerEnter(Collider col) {
		if (col.gameObject.CompareTag("BreakableObject")) {
			Vector3 dir = (col.transform.position - transform.position).normalized;
			dir.y = 0;
			Punch wave_punch = new Punch(dir, object_force, object_damage, 0, 0, HitClass.ITEM);
			BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
			if (bo) {
				bo.GetPunched(wave_punch);
			}
		} else if (!hit_puncher && col.gameObject.CompareTag("GhostPuncher")) {
			hit_puncher = true;
			Debug.Log("Wave Orb: I hit the ghost puncher!");
			GhostPuncher gp = col.gameObject.GetComponent<GhostPuncher>();
			if (gp) {
				Vector3 dir = (col.transform.position - transform.position).normalized;
				dir.y = 0;
				gp.GetPushed(dir, attrs.POWER);
			} else {
				Debug.LogError("Wave Orb: I hit an object with GhostPuncher tag but no ghost puncher component?");
			}
		}
	}
}
