using UnityEngine;
using System.Collections.Generic;

/* Maintains a record of which items have been purchased from the shop */
[System.Serializable]
public class ItemRecord {
	public List<Item> items;

	public ItemRecord() {
		items = new List<Item>();
	}

	public void AddItemByType(ItemType item_type) { AddItemByType(item_type, 0); }
	public void AddItemByType(ItemType item_type, int level) {
		switch (item_type) {
			case ItemType.SALT_SHAKER: AddItem(new SaltShaker()); break;
			case ItemType.PAINKILLERS: AddItem(new Painkillers()); break;
			case ItemType.GHOST_DETECTOR: AddItem(new GhostDetector()); break;
			case ItemType.PROTEIN_POWDER: AddItem(new ProteinPowder()); break;
			case ItemType.CIGARETTES: AddItem(new Cigarettes()); break;
			default:
				Debug.LogError("Attempted to add item type " + item_type + " but ItemRecord could not handle its instantiation.");
				break;
		}
	}


	public void AddItem(Item item) {
		// Does the item already exist? if so, replace its level with the incoming one
		Item existant = items.Find(i => i.item_type == item.item_type);
		if (existant != null) {
			Debug.Log("Item already exists: " + existant.item_type + " but at level " + existant.level + " (incoming " + item.level + ")" );
			// TODO: update existants level
		} else {
			Debug.Log("Adding item: " + item.item_type);
			items.Add(item);
		}
	}

	public void RemoveItem(Item item) {
	}
	
}

public class SerializedItemRecord {
	
}
