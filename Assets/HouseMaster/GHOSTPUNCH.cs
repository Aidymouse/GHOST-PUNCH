using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Overall game manager script **/
public class GHOSTPUNCH : MonoBehaviour
{
    void Start()
    {
    }

		IEnumerator GenerateHouse() {
			// TODO make this load the house scene and let it generate
			AsyncOperation l = SceneManager.LoadSceneAsync("LoadTestGhost", LoadSceneMode.Additive);
			while (!l.isDone) {
				yield return null;
			}

			Debug.Log("Loaded!");

			// I think ghost puncher can just be in this scene and we do some fancy cutscene to control stuff
		}


		public void StartRun () {
			// Remove the black background from the door
			// Start the timeline t
			Debug.Log("Starting Load");
			StartCoroutine(GenerateHouse());
		}

		public void EndRun() {
			// flash of white, teleport to shop scene (which I think will just never unload)
		}

}
