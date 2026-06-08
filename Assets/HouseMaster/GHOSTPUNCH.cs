using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

/** Overall game manager script **/
public class GHOSTPUNCH : MonoBehaviour
{

		bool house_ready;
		public PlayableDirector enter_house_timeline;

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
				/*
				GameObject ghost_container = GameObject.Find("SceneContainer");
				ghost_container.GetComponent<SceneContainer>().Enable();
				*/

					enter_house_timeline.Play();


			} else {
				Debug.Log("House is not ready yet!!!");
			}
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
