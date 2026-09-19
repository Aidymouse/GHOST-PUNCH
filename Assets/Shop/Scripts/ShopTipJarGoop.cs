using UnityEngine;

public class ShopTipJarGoop : MonoBehaviour
{
	Vector3 speed;
	Timer ti_life;

	void Awake() {
		speed = new Vector3(0, 0, 0);
		ti_life = new Timer(1, 1);
	}

	void Update() {
		transform.position += speed * Time.deltaTime;
		speed += Physics.gravity * Time.deltaTime;

		ti_life.Tick(Time.deltaTime);

		if (ti_life.Finished()) {
			Destroy(this.gameObject);
		}
	}
}
