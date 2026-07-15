using UnityEngine;

/** Handles events that need to be synchronised between scenes, mainly when going from shop to house 
 * How I handle scene management in Ghost Punch is each scene has a 'Master' script that has everything needed for managing that scene
 * For cross-scene management, the masters will call out to the scene manager who will distribute work to the scenes as needed
 * The idea is that ONLY 'Master' scripts will communicate with the scene manager. So you can count on everything else being routed through the scene itself
 * **/
public class GPSceneManager : MonoBehaviour
{
    HouseMaster house_master;
		ShopMaster shop_master;

		// Called by Genesis
		public void Init() {
			shop_master = GameObject.Find("ShopMaster").GetComponent<ShopMaster>();
			house_master = GameObject.Find("HouseMaster").GetComponent<HouseMaster>();
		}

		public void Signaled_EndStartRunCutscene() {
			house_master.SceneManaged_EndStartRunCutscene();
			shop_master.SceneManaged_EndStartRunCutscene();
		}

		public void EndRun() {
			house_master.SceneManaged_EndRun();
			shop_master.SceneManaged_EndRun();
		}

}
