using UnityEngine;

public class FootballCollider : MonoBehaviour {

	GhostPuncher p;
	public bool end_charge;
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

			//if (football_punch.object_damage < bo.hp || 
			// Maybe it should be based on weight???
			// it should... this doesn't let you break through doors....
			if (bo.GetBoundingBoxHeight() > p.defaults.CHARGE_MAX_HEIGHT) {
				Punch stopped_punch = new Punch(
					p.GetFacingDirection(),
					5000,
					500,
					300,
					500,
					(int)HitClass.PUNCH
				);
				bo.GetPunched(stopped_punch);
				Debug.Log("Hit something of height " + bo.GetBoundingBoxHeight());
				end_charge = true;
			} else {
				bo.GetPunched(football_punch);
			} 
		}

		Ghost ghost = col.gameObject.GetComponent<Ghost>();
		if (ghost) {
			Debug.Log("Hitting ghost wow!");
			Punch stopped_punch = new Punch(
				p.GetFacingDirection(),
				5000,
				500,
				300,
				500,
				(int)HitClass.PUNCH
			);
			ghost.GetPunched(stopped_punch);
			end_charge = true;
		}
	
	}

}

