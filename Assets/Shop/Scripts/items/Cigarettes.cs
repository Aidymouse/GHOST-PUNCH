
/* TODO */
public class Cigarettes : Item {
	public Cigarettes() : base(ItemType.CIGARETTES) {}
	public override void ApplyToGhost(Ghost ghost) {
		ghost.escape_needed += this.level * 5;
	}
}
