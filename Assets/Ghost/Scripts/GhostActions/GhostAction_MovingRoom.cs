

using UnityEngine;


public class GhostAction_MovingRoom : GhostAction {

	bool debug = false;
	public GhostAction_MovingRoom(Ghost g) : base(g) {}

	public override void Enter() {

		if (debug) {
			Debug.Log("Entering moving room. Nav agent active: " + ghost.nav_agent.isStopped + "; Nav destination: " + ghost.nav_destination);
		}

		if (ghost.nav_destination == null) {
			GameObject[] destinations = GameObject.FindGameObjectsWithTag("GhostDestination"); // Supposedly slow, but shouldn't be a big deal
			int dest_idx = Random.Range(0, destinations.Length);
			GameObject dest_obj = destinations[dest_idx];

			// If we've selected the area we're already at, the state fn will catch it

			ghost.nav_agent.destination = dest_obj.transform.position;
			ghost.nav_destination = dest_obj;

		} else {
			ghost.nav_agent.destination = ghost.nav_destination.transform.position;
		}
	}

	public override void Update() {
		if ((ghost.transform.position - ghost.nav_destination.transform.position).magnitude < 2) {
			ghost.nav_destination = null;
			ghost.EnterAction(GhostActions.CHARGING_ESCAPE);
		}
		// jumpscare sequence triggers at random for now - 1 in one hundred change each frame that the ghost becomes jumpscare enabled, then you have to get close enough to trigger it ???
		// This is kind of weird to do this here.
		if (Random.value < 0.01f)
		{
			ghost.jumpscareReady = true;
		}
	}

}
