using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Unity.Cinemachine;

public enum ShopSFX {
	MWAHAHA
}

public class Shop : MonoBehaviour
{
	const string SAVE_PREFIX = "GPSaveData_";

	CinemachineCamera[] cams;
	int cam_pointer;

	[Header("Sound")]
	public AudioSource shop_sound;
	public AudioClip sfx_start_run;

	[HideInInspector]
	public ItemRecord bought_items; // Loads from file

  void Start()
  {

		bought_items = new ItemRecord();
		
		// Cameras
		cam_pointer = 0;

		cams = GetComponentsInChildren<CinemachineCamera>();
		foreach (CinemachineCamera cam in cams) {
			cam.gameObject.SetActive(false);
		}
		cams[0].gameObject.SetActive(true);

  }

  void Update() { }

	/*** Camera Management ***/

	public void LookLeft() { 
		cams[cam_pointer].gameObject.SetActive(false);
		cam_pointer = (cam_pointer - 1) % cams.Length;
		cams[cam_pointer].gameObject.SetActive(true);
	}

	public void LookRight() {
		cams[cam_pointer].gameObject.SetActive(false);
		cam_pointer = (cam_pointer + 1) % cams.Length;
		cams[cam_pointer].gameObject.SetActive(true);
	}

	public void DisableCameras() {
		// All other cams should already be inactive
		cams[cam_pointer].gameObject.SetActive(false);
	}

	public void EnableCameras() {
		cams[cam_pointer].gameObject.SetActive(true);
	}




	/*** Item Management ***/

	public void BuyItem(ShopItem item) {
		Debug.Log(item.item_id + " costs " + item.cost + " ectoplasm");
		bought_items.AddItemByType(item.item_id, item.item_level);
	}

	public void LoadItemsFromFile() {
		// Open the file
		// TODO:
		// Parse the JSON
		// List<Item> saved_items = JsonUtility.FromJson(saved_str);
		// foreach (Item item of saved_items) { bought_items.AddItemByType(item.item_type, item.level); }
	}

	public void SaveItemsToFile(string profile_name) {
		string item_json = JsonUtility.ToJson(bought_items);
		Debug.Log("Item JSON: "+item_json);
		string save_filename = Application.persistentDataPath + "/" + SAVE_PREFIX + profile_name+".gpdata";
		Debug.Log("Item save file: "+save_filename);

		File.WriteAllText(save_filename, item_json);
	}

	public void PopulateShop() {
		/** load item prefabs into shop slots **/
		// Position item correctly
		// Link up clicking item to the right method (my BuyItem method)
	}

	public void PlaySound(ShopSFX sfx) {
		switch (sfx) {
			case (ShopSFX.MWAHAHA): {
				shop_sound.clip = sfx_start_run;
				shop_sound.Play();
				break;
			}
		}
	}

}

