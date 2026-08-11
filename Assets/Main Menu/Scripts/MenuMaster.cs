using UnityEngine;

public enum Menus {
	MAIN,
	PROFILES
}

public class MenuMaster : MonoBehaviour
{

		// UIs so we can set them on and off
		public GameObject main_menu;
		public GameObject profile_menu;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


		void DisableAllUIs() {
			main_menu.SetActive(false);
			profile_menu.SetActive(false);
		}

		public void SwitchToMenu(string menu) {
			DisableAllUIs();
			switch (menu) {
				case "main": main_menu.SetActive(true); break;
				case "profiles": profile_menu.SetActive(true); break;
			}
		}

		void EnableProfileUI() {
			profile_menu.SetActive(true);
		}

		/** Main Menu Methods **/
		public void Quit() {
			Application.Quit();
		}

		/** Profile Menu Methods **/
		public void LoadProfile(int profile_number) {
			Debug.Log("Loading profile " + profile_number);
		}

		/* TODO */
		public void DeleteProfile() {
		}

}
