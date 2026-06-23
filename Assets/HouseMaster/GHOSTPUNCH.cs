using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.Cinemachine;

/** Game state manager script **/
public class GHOSTPUNCH : MonoBehaviour
{

		bool house_ready;
		public PlayableDirector enter_house_timeline;
		public PlayableDirector end_run_timeline;

		public CinemachineCamera VCam_MouseControlled;
		public CinemachineCamera VCam_Shop;

		/* House time stuff */
		public GhostPuncher puncher_instance;
		public Ghost ghost_instance;
		public GhostUI ghost_ui;
		public ShopDoor shop_door;

		/* Also contains the data for what items are present */
		public Shop shop;

    void Start()
    {
			house_ready = false;
			StartCoroutine(LoadHouseScene());
    }

		IEnumerator LoadHouseScene() {
			// TODO make this load the house scene and let it generate
			AsyncOperation l = SceneManager.LoadSceneAsync("LoadTestGhost", LoadSceneMode.Additive);
			while (!l.isDone) {
				yield return null;
			}

			house_ready = true;

		}


		public void StartRun () {
			Debug.Log("Start Run");
			// Remove the black background from the door
			// Start the timeline t
			if (house_ready) {
				Debug.Log("House is ready!");

				// Enable house scene
				GameObject ghost_container = GameObject.Find("SceneContainer");
				ghost_container.GetComponent<SceneContainer>().Enable();

				shop_door.StartRun();
			
				Debug.Log(shop.bought_items);
				puncher_instance.ApplyItems(shop.bought_items);
				ghost_instance.ApplyItems(shop.bought_items);
				// TODO: // ghost_instance.ApplyUtems(shop.bought_items);

				// SIGNAL: this cutscene triggers a signal
				enter_house_timeline.Play();
				

			} else {
				Debug.Log("House is not ready yet!!!");
			}
		}

		public void Signaled_EndStartRunCutscene() {
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
