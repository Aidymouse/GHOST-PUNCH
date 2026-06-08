public enum ItemType {
	PROTEIN_POWDER
}

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
	//public virtual void ApplyToHouseMaster( ghost_puncher) {}

	// INFO: idea. Means we can get material based on level. But it might be funnier to just 
	//public GetMaterial() { }
	
}

/* */
public class ProteinPowder : Item {

	public ProteinPowder() : base(ItemType.PROTEIN_POWDER) {}

	public override void ApplyToGhostPuncher(GhostPuncher ghost_puncher) {
		ghost_puncher.max_stamina = ghost_puncher.defaults.BASE_STAMINA * 1 + (0.2f * this.level);
	}
	
}
