using UnityEngine;
using UnityEngine.SceneManagement;

/** Root scene that starts the game, loading various things (and maybe linking them together?) 
 * Configurable for debug
 * **/
public class GPLoader : MonoBehaviour
{

		public GPSettings settings;
		public GPSceneManager scene_manager;
	
		public bool load_shop;
		public bool load_house;

		HouseMaster house_master;
		ShopMaster shop_master;

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

			// Load preferences
			if (!PlayerPrefs.HasKey("first_load") || settings.force_reload_settings == true) {
				Debug.Log("Setting prefs!");
				InitPrefs();
			}
			LoadPrefs();

			// Init scene load
			// TODO: this will have to be the menu some day
			if (load_shop) { SceneManager.LoadScene("Shop"); }
			if (load_house) { SceneManager.LoadScene("House"); }

			// Init the scene manager so it can do it's stitching
			scene_manager.Init();



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
