using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.Cinemachine;

/** Game state manager script, lives in the shop. But handles the transition to and from the house scene **/
public class ShopMaster : MonoBehaviour
{

		public PlayableDirector enter_house_timeline;

		public PlayableDirector start_end_run_timeline;
		public PlayableDirector end_end_run_timeline;

		public CinemachineCamera VCam_MouseControlled;
		public CinemachineCamera VCam_Shop;

		GPSceneManager scene_manager;

		/* House time stuff */
		public GhostPuncher puncher_instance;
		public Ghost ghost_instance;
		public GhostUI ghost_ui;
		public ShopUI shop_ui;
		public ShopDoor shop_door;

		/* Also contains the data for what items are present */
		public Shop shop;

    void Start()
    {
			GameObject scene_manager_root = GameObject.Find("SceneManager");
			scene_manager = scene_manager_root.GetComponent<GPSceneManager>();
    }

		/** START RUN **/

		/** A.k.a StartStartRunCutscene */
		public void StartRun () {
			Debug.Log("Start Run");

			shop_door.StartRun();
			shop_ui.StartRun();

			shop.PlaySound(ShopSFX.MWAHAHA);
		
			Debug.Log(shop.bought_items);
			puncher_instance.ApplyItems(shop.bought_items);
			ghost_instance.ApplyItems(shop.bought_items);
			// TODO: // ghost_instance.ApplyUtems(shop.bought_items);

			// SIGNAL: this cutscene triggers a signal
			enter_house_timeline.Play();

			puncher_instance.ChangeAnimation("Entrance");

			Cursor.lockState = CursorLockMode.Locked;
				

		}

		/** 
 		* We want timeline to trigger scene manager, but it lives in a different scene!
 		* This method lets the timeline call it through ShopMaster
 		*/
		public void Signaled_CurryEndStartRunCutscene() {
			scene_manager.EndStartRunCutscene();
		}

		public void SceneManaged_EndStartRunCutscene() {
			Debug.Log("Start Run - Signal received");
			shop.DisableCameras();

			ghost_instance.StartRun();
			puncher_instance.StartRun();

			ghost_ui.gameObject.SetActive(true);
			ghost_ui.InitUI(ghost_instance, puncher_instance);

		}

		public void Signaled_TriggerPuncherAnimation(string animation) {
			puncher_instance.ChangeAnimation(animation);
		}

		/** END RUN - START */
		public void CurryStartEndRun() {
			scene_manager.StartEndRun();
		}

		public void SceneManaged_StartEndRun() {
			start_end_run_timeline.Play();

			ghost_ui.gameObject.SetActive(false);

			shop_door.EndRun();
			shop_ui.EndRun();

			ghost_instance.EndRun();
			puncher_instance.EndRun();
			puncher_instance.ChangeAnimation("KickedOutStart");

			Cursor.lockState = CursorLockMode.None;

			ghost_ui.EndRun();
		}


		public void Signaled_PauseEndRunEffects() {
			start_end_run_timeline.Pause();
		}

		/*** END RUN - END **/
		public void SceneManaged_EndEndRun() {
			start_end_run_timeline.Stop();
			end_end_run_timeline.Play();
			puncher_instance.ChangeAnimation("KickedOutEnd");
			shop.EnableCameras();
		}

}
