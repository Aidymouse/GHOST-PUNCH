using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
  public float turnSpeed;
  public Quaternion turnGoal;

  public Transform camera_pos;
  public Transform camera_target;

	[HideInInspector]
	public ItemRecord bought_items;

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

	public void PopulateShop() {
	}

}
