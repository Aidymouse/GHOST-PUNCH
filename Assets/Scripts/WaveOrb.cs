using UnityEngine;

public class WaveOrb : MonoBehaviour
{
    public float expansion_speed;
    public ParticleSystem wave_particles;
    public Vector3 particle_offset;
    public float life_timer;
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
}
