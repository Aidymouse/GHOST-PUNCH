using UnityEngine;
public class FootballWallCollider : MonoBehaviour {
	GhostPuncher p;
	public bool end_charge;

	public void Awake() {
		p = GetComponentInParent<GhostPuncher>();
	}
	
	public void OnTriggerEnter(Collider col) {
		// TODO:
	}
}

