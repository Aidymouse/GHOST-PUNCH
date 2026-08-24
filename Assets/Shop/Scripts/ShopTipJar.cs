using UnityEngine;

public class ShopTipJar : MonoBehaviour
{
	public ParticleSystem particles;

	public void OnClick() {
		particles.Play();
	}
}
