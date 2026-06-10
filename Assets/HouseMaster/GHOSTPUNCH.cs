using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.Cinemachine;

/** Overall game manager script **/
public class GHOSTPUNCH : MonoBehaviour
{

		bool house_ready;
		public PlayableDirector enter_house_timeline;

		public CinemachineCamera VCam_MouseControlled;
		public CinemachineCamera VCam_Shop;

		/* House time stuff */
		public GhostPuncher puncher_instance;
		public Ghost ghost_instance;
		public GhostUI ghost_ui;
		public ShopDoor shop_door;

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

			// I think ghost puncher can just be in this scene and we do some fancy cutscene to control stuff
		}


		public void StartRun () {
			// Remove the black background from the door
			// Start the timeline t
			if (house_ready) {
				Debug.Log("House is ready!");

				// Enable house scene
				GameObject ghost_container = GameObject.Find("SceneContainer");
				ghost_container.GetComponent<SceneContainer>().Enable();

				shop_door.StartRun();

				// SIGNAL: this cutscene triggers a signal
				enter_house_timeline.Play();
				


			} else {
				Debug.Log("House is not ready yet!!!");
			}
		}

		public void Signaled_EndStartRunCutscene() {
			Debug.Log("Signal received");
			ghost_ui.gameObject.SetActive(true);
			VCam_Shop.gameObject.SetActive(false);

			ghost_instance.StartRun();
			puncher_instance.StartRun();
		}

		public void EndRun() {
			// flash of white, teleport to shop scene (which I think will just never unload)
			//
			// Flash of white
			// TODO: UI code
			//
			// Teleport to shop scene
			// 1. Unload the player camera
			// 2. Set player camera back to shop camera (or maybe to a 'kicked out' timeline)
		}

}
