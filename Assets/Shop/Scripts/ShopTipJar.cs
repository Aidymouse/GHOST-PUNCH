using UnityEngine;

public class ShopTipJar : MonoBehaviour
{
	public ParticleSystem particles;
	public GameObject goop_obj;
	public Transform goop_spawn;

	public void OnClick() {
		particles.Play();

		SpawnGoop();
	}


	public void SpawnGoop() {
		GameObject new_goop = Instantiate(goop_obj, this.transform);
		new_goop.transform.position = goop_spawn.position;
	}
	
}
