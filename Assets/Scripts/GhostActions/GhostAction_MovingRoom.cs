

using UnityEngine;

public class GhostAction_MovingRoom : GhostAction {

	public GhostAction_MovingRoom(Ghost g) : base(g) {}

	public override void Update() {
		if ((ghost.transform.position - ghost.nav_destination.transform.position).magnitude < 2) {
			ghost.nav_destination = null;
			ghost.EnterAction(Ghost.GhostActions.CHARGING_ESCAPE);
		}
		// jumpscare sequence triggers at random for now
		if (Random.value < 0.01f)
		{
			ghost.jumpscareReady = true;
		}
	}
	
}
