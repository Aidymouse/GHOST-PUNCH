
using UnityEngine;

public class GhostAction_PeakAroundCorner : GhostAction {

	public GhostAction_PeakAroundCorner(Ghost g) : base(g) {}

	public override void Enter() {
	}

	public override void Exit() {
	}

	public override void Update() {
	}

	/* Find the corners near the ghost puncher */
	void FindCorners() {
		// 1. They need to be within the ghost punchers vicinity. And probably in the direction he's looking.
		// For now we'll just use door pieces.
		// We also need pieces where the doors are missing.
		
		GameObject[] peakables = GameObject.FindGameObjectsWithTag("Peakable");
	}
	
}




