using UnityEngine;
using UnityEngine.SceneManagement;

/** Root scene that starts the game, loading various things (and maybe linking them together?) 
 * Configurable for debug
 * **/
public class GPLoader : MonoBehaviour
{

		public GPSettings settings;

		public string init_scene;

		void InitPref(string key, float p) {
			Debug.Log("Setting pref for '" + key + "': "+p);
			PlayerPrefs.SetFloat(key, p);
		}

		/** First time player prefs set up **/
		void InitPrefs() {
			InitPref("mouse_sensitivity", settings.mouse_sensitivity);
			PlayerPrefs.SetInt("first_load", 0);
		}


		/** Returning player pref set up **/
		void LoadPrefs() {
			settings.mouse_sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity");
		}

    void Start()
    {

			if (!PlayerPrefs.HasKey("first_load") || settings.force_reload_settings == true) {
				Debug.Log("Setting prefs!");
				InitPrefs();
			}
			LoadPrefs();
			
        

			if (init_scene != "") {
				SceneManager.LoadScene(init_scene);
			} else {
				Debug.LogWarning("You're not going to load a scene?");
			}



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
