using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AI;

/** The escape goblins only job is to steal the camera off of ghost puncher and
 * pathfind to the exit of the house when the run ends. */
public class EscapeGoblin : MonoBehaviour
{
	public HouseMaster house_master;

	public float escape_timeout;
	public Timer ti_escape_timeout;
	NavMeshAgent nav_agent;
	public CinemachineCamera VCam_Escaper;
	public Transform house_exit;

	public void Awake() {
		ti_escape_timeout = new Timer(escape_timeout, escape_timeout);
		nav_agent = GetComponent<NavMeshAgent>();
	}

	public void Update() {
		ti_escape_timeout.Tick(Time.deltaTime);
		if (ti_escape_timeout.FinishedThisFrame()) {
			// TODO: Fade out
			Debug.Log("Escape timed out");
		}

		Vector3 dist = house_exit.position - transform.position;
		dist.y = 0;
		if (dist.magnitude < 0.2) {
			Debug.Log("Close enough to exit trigger");
		}
		
	}

	public void OnTriggerEnter(Collider col) {
		if (col.gameObject == house_exit.gameObject) {
			Debug.Log("Goblin has found the exit");
			house_master.CurryEndEndRun();
		}
	}

	public void StartEscaping(Vector3 pos, Vector3 facing_rotation) {
			nav_agent.transform.position = pos;
			nav_agent.transform.localEulerAngles = facing_rotation;
			nav_agent.destination = house_exit.position;

			ti_escape_timeout.Reset();
	}

	public void StopEscaping() {
		// Passthrough right now. Might change
	}
}
