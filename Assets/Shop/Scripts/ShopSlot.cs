
using UnityEngine;
using System;


/* Script that is a slot in the shop where an item can be. Interfaces between shop UI and shop slot.
 * Generally controls the ShopItem
 * */
public class ShopSlot : MonoBehaviour {

	[HideInInspector] public ShopItem item;
	public ShopPrefabs prefabs;
	
	[Header("Debug")]
	public bool spawn_item;
	public ItemType init_item;
	public ParticleSystem purchase_particles;

	public void Awake() {
		item = GetComponentInChildren<ShopItem>();

		if (spawn_item) {
			GameObject shop_item = Instantiate(GetPrefabForItemType(init_item), this.transform);
			item = shop_item.GetComponent<ShopItem>();
		}
	}
		
	/* Item Interaction */
	public void MouseOver() {
		if (item) {
			item.StartSpinning();
		}
	}

	public void MouseOut() {
		if (item) {
			item.StopSpinning();
		}
	}
		

	public void MouseDown() {
		// TODO: spawn particles
		if (item) {
			item.ShrinkAndDisappear();
			if (purchase_particles) {
				Instantiate(purchase_particles, this.transform);
			}
		}
	}

	/* Item Management */
	public void StockWithItem(Item item) {
		// Get the shop item prefab and put into my slot all nice
	}


	public GameObject GetPrefabForItemType(ItemType item_type) {
		GameObject prefab = prefabs.item_prefabs[(int)item_type];

		
		if (!prefab) {
			Debug.LogError("Trying to get the ShopItem prefab for" + item_type + ", but found no prefab. Is it in the ShopPrefabs object?");
			throw new Exception();
		}

		ShopItem shop_item = prefab.GetComponent<ShopItem>();

		if (!shop_item) {
				Debug.LogError("Shop item prefab " + prefab + " does not appear to have shop item component...");
				throw new Exception();
		}

		if (!(shop_item.item_id == item_type)) {
			Debug.LogWarning("Shop item prefab was found for type " + item_type + ", but does not appear to actually have that type. Need to update ShopPrefabs?");
		}

		return prefab;


	}


}
