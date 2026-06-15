using UnityEngine;

public enum ItemType {
	PROTEIN_POWDER,
	SALT_SHAKER,
	PAINKILLERS,
	GHOST_DETECTOR,
}

public class Item {
	
	// Single use items only have one level
	public ItemType item_type;
	public int level;

	bool applies_to_puncher;
	bool applies_to_ghost;
	bool applies_to_house;

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

/* */
public class ProteinPowder : Item {

	public ProteinPowder() : base(ItemType.PROTEIN_POWDER) {}

	public override void ApplyToGhostPuncher(GhostPuncher ghost_puncher) {
		Debug.Log("Applying protein powder!");
		ghost_puncher.max_stamina = ghost_puncher.defaults.BASE_STAMINA * 1 + (0.2f * this.level);
	}
	
}

public class SaltShaker : Item {
	public SaltShaker() : base(ItemType.SALT_SHAKER) {}
}

public class Painkillers : Item {
	public Painkillers() : base(ItemType.PAINKILLERS) {}
}

public class GhostDetector : Item {
	public GhostDetector() : base(ItemType.GHOST_DETECTOR) {}
}
