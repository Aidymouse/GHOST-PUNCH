using UnityEngine;

public class HouseMaster : MonoBehaviour
{

		public GameObject enabled_on_run_start;

		public void SceneManaged_EndStartRunCutscene() {
			enabled_on_run_start.SetActive(true);
		}

		public void SceneManaged_EndRun() {
			// TODO: at some point i'll need to make sure this only happens when we can't see it
			enabled_on_run_start.SetActive(false);
		}
}
