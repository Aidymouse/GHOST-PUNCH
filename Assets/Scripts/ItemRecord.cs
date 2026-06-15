using UnityEngine;
using System.Collections.Generic;

/* Maintains a record of which items have been purchased from the shop */
public class ItemRecord {
	public List<Item> items;

	public ItemRecord() {
		items = new List<Item>();
	}

	public void LoadFromFile() {
		// TODO: populate items array
	}
	public void SaveToFile() {}

	public void AddItem(Item item) {
		// Does the item already exist? if so, replace its level with the incoming one
		//items.Add(item);
		Debug.Log("Adding item: " + item.item_type);
		items.Add(item);
	}

	public void RemoveItem(Item item) {
	}
	
}
