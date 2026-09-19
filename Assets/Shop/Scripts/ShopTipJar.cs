using UnityEngine;

public class ShopTipJar : MonoBehaviour
{
	public GameObject particles_obj;
	public Transform particles_spawn;

	public GameObject goop_obj;
	public Transform goop_spawn;

	public void OnClick() {
		SpawnGoop();
	}


	public void SpawnGoop() {
		GameObject new_goop = Instantiate(goop_obj, this.transform);
		new_goop.transform.position = goop_spawn.position;

		GameObject new_particles = Instantiate(particles_obj);
		new_particles.transform.position = particles_spawn.position;
	}
	
}
