using UnityEngine;

public class GhostAction_ChargingEscape : GhostAction {

	public GhostAction_ChargingEscape(Ghost g) : base(g) {}

	public override void Update() {
		float old_escape = ghost.escape_meter;
		bool prevEscaped = ghost.Escaped();
		ghost.escape_meter += Time.deltaTime;

		if (!prevEscaped && ghost.Escaped()) {
			ghost.ghostPuncher.GetComponent<GhostPuncher>().EndRun();
		}
	}
	
}
