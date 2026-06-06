public enum ItemType {
	PROTEIN_POWDER;
}

public class Item {
	
	// Single use items only have one level
	ItemType type;
	int level;

	public Item(ItemType type) {
		id = item_id;
		level = 0;
	}

	public virtual void ApplyToGhost(Ghost ghost) {}
	public virtual void ApplyToGhostPuncer(GhostPuncher ghost_puncher) {}
	//public virtual void ApplyToHouseMaster( ghost_puncher) {}

	// INFO: idea. Means we can get material based on level. But it might be funnier to just 
	public GetMaterial() { }
	
}

/* */
public class ProteinPowder : Item {

	public ProteinPowder() : base() {}

	public override void ApplyToGhostPuncher() {
		ghost_puncher.max_stamina = ghost_puncher.defaults.BASE_STAMINA * 1 + (0.2 * this.level);
	}
	
}
