using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.AI;

public class HouseMaster : MonoBehaviour
{

		public GameObject enabled_on_run_start;
		public Transform house_exit;
		public EscapeGoblin escape_goblin;

		GhostPuncher puncher;
		GPSceneManager scene_manager;


		void Awake() {
			GameObject scene_manager_root = GameObject.Find("SceneManager");
			scene_manager = scene_manager_root.GetComponent<GPSceneManager>();
		}

		void Start() {
			// Not a huge fan of this, but how else !?
			puncher = GameObject.Find("GHOST PUNCHER").GetComponent<GhostPuncher>();
		}


		public void SceneManaged_EndStartRunCutscene() {
			enabled_on_run_start.SetActive(true);
		}

		/** START END RUN **/
		public void SceneManaged_StartEndRun() {
			// TODO: at some point i'll need to make sure this only happens when we can't see it
			enabled_on_run_start.SetActive(false);

			escape_goblin.gameObject.SetActive(true);
			escape_goblin.StartEscaping(puncher.transform.position, new Vector3(0, puncher.transform.localEulerAngles.y+180, 0));

			// TODO: play the StartEndRun timeline
		}

		/** END END RUN */
		public void CurryEndEndRun() {
			scene_manager.EndEndRun();
		}

		public void SceneManaged_EndEndRun() {
			escape_goblin.StopEscaping();
			escape_goblin.gameObject.SetActive(false);
		}

}
