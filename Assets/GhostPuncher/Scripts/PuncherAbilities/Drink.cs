
using UnityEngine;

public class Drink : PuncherAbility {

	public Drink(GhostPuncher p) : base(p) { }

	public override void EnterAbility() {
		puncher.ChangeAnimation("Drink");
		puncher.ExitAbility();
	}

	


}
