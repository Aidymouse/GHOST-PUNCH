
using UnityEngine;

public class ForceVisualiser : MonoBehaviour {

	public Rigidbody rb;

	public Color color;

	public void Start() {
		if (rb == null) {
			rb = this.gameObject.GetComponent<Rigidbody>();
		}
		if (rb == null) {
			rb = this.gameObject.GetComponentInChildren<Rigidbody>();
		}
	}

	public void Update() {
		Vector3 vel = rb.linearVelocity;
		Debug.DrawRay(rb.position, vel, color);
	}
	

}
