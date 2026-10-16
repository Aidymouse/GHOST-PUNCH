public class GhostPathing : MonoBehaviour {

	NavMeshAgent nav_agent;
	Ghost ghost;
	public bool isStopped;

	public float startTurning;

	public void Awake() {
		ghost = GetComponent<Ghost>();
		nav_agent = ghost.nav_agent;
	}

	/** Grr.... */
	public void FixedUpdate() {
		// Do we have a path? Follow it!
		// Are we getting close to a corner? Rotate towards the *next* corner.
	}

}
