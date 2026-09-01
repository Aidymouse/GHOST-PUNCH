using UnityEngine;
public class FootballWallCollider : MonoBehaviour {
	GhostPuncher p;
	public bool end_charge;

	public void Awake() {
		p = GetComponentInParent<GhostPuncher>();
	}
	
	public void OnTriggerEnter(Collider col) {
		// TODO:
		if (col.gameObject.CompareTag("Wall")) {
			end_charge = true;
		}
	}

	/*
	public void OnCollisionEnter(Collision coli) {
		// TODO:
		if (coli.gameObject.CompareTag("Wall")) {
			end_charge = true;
		}
	}
	*/
}

