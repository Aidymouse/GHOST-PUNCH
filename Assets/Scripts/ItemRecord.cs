using UnityEngine;
/* Maintains a record of which items have been purchased from the shop */
public class ItemRecord {
	public Item[] items;

	public void LoadFromFile() {
		// TODO: populate items array
	}
	public void SaveToFile() {}

	public void AddItem(Item item) {
		// Does the item already exist? if so, replace its level with the incoming one
		//items.Add(item);
		Debug.Log("Adding item: " + item.item_type);
	}

	public void RemoveItem(Item item) {
	}
	
}
