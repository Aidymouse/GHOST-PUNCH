using UnityEngine;

public class GhostAction_ChargingEscape : GhostAction {

	public GhostAction_ChargingEscape(Ghost g) : base(g) {}

	public override void Enter() {
				// TODO: If we can see the player (i.e. they kept pace with us well), skip straight to choosing a power.
				ghost.charge_particles.Play();
				ghost.ChangeAnimation("ChargeEscape");
	}

	public override void Exit() {
				ghost.charge_particles.Stop();
	}

	public override void Update() {
		// TODO: be making wubwubwubwubwubwubwub sound
		ghost.escape_meter += Time.deltaTime;

		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}
	
}
