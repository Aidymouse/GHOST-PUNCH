using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Cinemachine;

public enum ShopSFX {
	MWAHAHA
}

public class Shop : MonoBehaviour
{
	const string SAVE_PREFIX = "GPSaveData_";

	public List<CinemachineCamera> cams;

	[Header("Sound")]
	public AudioSource shop_sound;
	public AudioClip sfx_start_run;

	[HideInInspector]
	public ItemRecord bought_items; // Loads from file

  void Start()
  {

		bought_items = new ItemRecord();

    camera_target.position = camera_pos.position + Vector3.forward * 10;

  }

  void Update() { }

  public void LookTowardsShop() { }

  public void LookTowardsDoor() { }

	/* Item Management */
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

