using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.Cinemachine;

/** Game state manager script, lives in the shop. But handles the transition to and from the house scene **/
public class ShopMaster : MonoBehaviour
{

		public PlayableDirector enter_house_timeline;
		public PlayableDirector end_run_timeline;

		public CinemachineCamera VCam_MouseControlled;
		public CinemachineCamera VCam_Shop;

		GPSceneManager scene_manager;

		/* House time stuff */
		public GhostPuncher puncher_instance;
		public Ghost ghost_instance;
		public GhostUI ghost_ui;
		public ShopDoor shop_door;

		/* Also contains the data for what items are present */
		public Shop shop;

    void Start()
    {
			GameObject scene_manager_root = GameObject.Find("SceneManager");
			scene_manager = scene_manager_root.GetComponent<GPSceneManager>();
    }

		/*
		IEnumerator LoadHouseScene() {
			// TODO make this load the house scene and let it generate
			AsyncOperation l = SceneManager.LoadSceneAsync(house_scene, LoadSceneMode.Additive);
			while (!l.isDone) {
				yield return null;
			}

			house_ready = true;

		}
		*/


		public void StartRun () {
			Debug.Log("Start Run");

			shop_door.StartRun();
		
			Debug.Log(shop.bought_items);
			puncher_instance.ApplyItems(shop.bought_items);
			ghost_instance.ApplyItems(shop.bought_items);
			// TODO: // ghost_instance.ApplyUtems(shop.bought_items);

			// SIGNAL: this cutscene triggers a signal
			enter_house_timeline.Play();
				

		}

		public void Signaled_CurryEndStartRunCutscene() {
			scene_manager.Signaled_EndStartRunCutscene();
		}

		public void SceneManaged_EndStartRunCutscene() {
			Debug.Log("Start Run - Signal received");
			VCam_Shop.gameObject.SetActive(false);

			ghost_instance.StartRun();
			puncher_instance.StartRun();

			ghost_ui.gameObject.SetActive(true);
			ghost_ui.InitUI(ghost_instance, puncher_instance);

		}

		public void EndRun() {
			Debug.Log("Ending Run!");
			puncher_instance.GetComponent<GhostPuncher>().EndRun();
			VCam_Shop.gameObject.SetActive(true);

			// SIGNAL: triggers below Fn
			end_run_timeline.Play();
		}

		// NOTE: the end run timeline will handle the enablement of the lose UI
		public void Signaled_EndRunWhiteOpaque() {
			Debug.Log("Ending Run - received signal");
			ghost_ui.gameObject.SetActive(false);
			shop_door.EndRun();

			ghost_instance.EndRun();

			Cursor.lockState = CursorLockMode.None;

			ghost_ui.EndRun();
		}

}
