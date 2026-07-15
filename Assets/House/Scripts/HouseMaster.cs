using UnityEngine;

public class HouseMaster : MonoBehaviour
{

		public GameObject enabled_on_run_start;

		public void SceneManaged_EndStartRunCutscene() {
			enabled_on_run_start.SetActive(true);
		}

		public void EndRun() {
		}
}
