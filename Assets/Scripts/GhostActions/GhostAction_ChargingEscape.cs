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
		float old_escape = ghost.escape_meter;
		bool prevEscaped = ghost.Escaped();
		ghost.escape_meter += Time.deltaTime;

		if (!prevEscaped && ghost.Escaped()) {
			ghost.ghostPuncher.GetComponent<GhostPuncher>().EndRun();
		}
		// Can I see the player? Have I seen them for some amount of timer? Startle!
	}
	
}
