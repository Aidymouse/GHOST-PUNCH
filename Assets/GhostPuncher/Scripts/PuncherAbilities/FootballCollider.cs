using UnityEngine;

public class FootballCollider : MonoBehaviour {

	GhostPuncher p;
	public void Awake() {
		p = GetComponentInParent<GhostPuncher>();
	}

	public void OnTriggerEnter(Collider col) {

		BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
		if (bo) {
			Debug.Log("Hit something breakable!");

			Punch football_punch = new Punch(
				p.GetFacingDirection(),
				1000,
				500,
				300,
				500,
				(int)HitClass.PUNCH
			);

			bo.GetPunched(football_punch);
		}
	
	}

}
