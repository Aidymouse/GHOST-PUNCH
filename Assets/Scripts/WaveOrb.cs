using UnityEngine;

public class WaveOrb : MonoBehaviour
{
	public float expansion_speed;
	public ParticleSystem wave_particles;
	public Vector3 particle_offset;
	public float life_timer;

	public GhostPowerAttribs attrs;

	public float object_damage;
	public float object_force;

	Timer life;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		life = new Timer(life_timer);

		Instantiate(wave_particles, transform.position + particle_offset, transform.rotation);
	}

	// Update is called once per frame
	void Update()
	{

		float expansion = expansion_speed * Time.deltaTime;

		transform.localScale += new Vector3(expansion, expansion, expansion);

		life.tick(Time.deltaTime);
		if (life.finished()) {
			Destroy(this.gameObject);
		}

	}

	void OnTriggerEnter(Collider col) {

		
		if (col.gameObject.CompareTag("BreakableObject")) {
			Vector3 dir = (col.transform.position - transform.position).normalized;
			dir.y = 0;
			Punch wave_punch = new Punch(dir, object_force, object_damage, 0, 0, 5);
			BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
			if (bo) {
				bo.GetPunched(wave_punch);
			}
		} else if (col.gameObject.CompareTag("GhostPuncher")) {
			Debug.Log("I hit the ghost puncher!");
				GhostPuncher gp = col.gameObject.GetComponent<GhostPuncher>();
				if (gp) {
						Vector3 dir = (col.transform.position - transform.position).normalized;
						dir.y = 0;
						gp.GetPushed(dir, attrs.WAVE_POWER);
				}
		}
	}
}
