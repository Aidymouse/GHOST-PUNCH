using UnityEngine;
using System.Collections.Generic;

public enum ItemType {
	PROTEIN_POWDER=0,
	SALT_SHAKER=1,
	PAINKILLERS=2,
	GHOST_DETECTOR=3,
	CIGARETTES=4,
}

[System.Serializable]
public class Item {
	
	// Single use items only have one level
	public ItemType item_type;
	public int level;

	public Item(ItemType new_type) {
		item_type = new_type;
		level = 0;
	}

	public virtual void ApplyToGhost(Ghost ghost) {}
	public virtual void ApplyToGhostPuncher(GhostPuncher ghost_puncher) {}

	public void SetLevel(int new_level) {
		level = new_level;
	}
	//public virtual void ApplyToHouseMaster( ghost_puncher) {}

	// INFO: idea. Means we can get material based on level. But it might be funnier to just 
	//public GetMaterial() { }
	
}





