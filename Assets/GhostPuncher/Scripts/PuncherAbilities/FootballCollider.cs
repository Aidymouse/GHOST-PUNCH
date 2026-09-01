using UnityEngine;

public class FootballCollider : MonoBehaviour {

	GhostPuncher p;
	public bool end_charge;
	bool hit_ghost_this_frame;
	public void Awake() {
		p = GetComponentInParent<GhostPuncher>();
		hit_ghost_this_frame = false;
	}

	public void Update() {
		hit_ghost_this_frame = false;
	}

	public void OnTriggerEnter(Collider col) {

		BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
		if (bo) {

			//if (football_punch.object_damage < bo.hp || 
			// Maybe it should be based on weight???
			// it should... this doesn't let you break through doors....
			if (bo.GetBoundingBoxHeight() > p.defaults.CHARGE_MAX_HEIGHT) {
				Punch stopped_punch = Punch.FromData(p.GetFacingDirection(), p.defaults.CHARGE_PUNCH_STOP);
				bo.GetPunched(stopped_punch);
				//Debug.Log("Hit something of height " + bo.GetBoundingBoxHeight());
				end_charge = true;
			} else {
				Punch trample_punch = Punch.FromData(p.GetFacingDirection(), p.defaults.CHARGE_PUNCH_TRAMPLE);
				bo.GetPunched(trample_punch);
			} 
		} else {
			Debug.Log(col.gameObject);
		}

		// Do frame check when hit ghost because we're hitting a rigidbody collider and we could be hitting a bunch of them at once
		// I think we may have a problem where we can hit the ghost more than once.... TODO:
		if (!hit_ghost_this_frame) {
			Ghost ghost = col.gameObject.GetComponentInParent<Ghost>();
			if (ghost) {
				hit_ghost_this_frame = true;
				//Debug.Log("Hitting ghost wow!");
				Punch stopped_punch = Punch.FromData(p.GetFacingDirection(), p.defaults.CHARGE_PUNCH_STOP);
				ghost.GetPunched(stopped_punch);
				end_charge = true;
			}
		}
	
	}

}

