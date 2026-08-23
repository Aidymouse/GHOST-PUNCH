using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AI;

public class HouseMaster : MonoBehaviour
{

		public GameObject enabled_on_run_start;
		public NavMeshAgent escaper;
		public CinemachineCamera VCam_Escaper;
		public Transform house_exit;

		GhostPuncher puncher;

		void Start() {
			// Not a huge fan of this, but how else !?
			puncher = GameObject.Find("GHOST PUNCHER").GetComponent<GhostPuncher>();
		}

		public void SceneManaged_EndStartRunCutscene() {
			enabled_on_run_start.SetActive(true);
		}

		public void SceneManaged_EndRun() {
			// TODO: at some point i'll need to make sure this only happens when we can't see it
			enabled_on_run_start.SetActive(false);

			// TODO: move the house escaper to the right spot, switch the VCams, and set the location
			escaper.transform.position = puncher.transform.position;
			escaper.transform.localEulerAngles = new Vector3(0, puncher.transform.localEulerAngles.y, 0);

			escaper.gameObject.SetActive(true);
			escaper.destination = house_exit.position;

		}

		public void EscaperContactedExit() {
		}
}
