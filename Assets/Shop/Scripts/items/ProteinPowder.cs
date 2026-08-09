using UnityEngine;

/* */
public class ProteinPowder : Item {

	public ProteinPowder() : base(ItemType.PROTEIN_POWDER) {}

	public override void ApplyToGhostPuncher(GhostPuncher ghost_puncher) {
		Debug.Log("Applying protein powder!");
		ghost_puncher.max_stamina = ghost_puncher.defaults.BASE_STAMINA * 1 + (0.2f * this.level);
	}
	
}
