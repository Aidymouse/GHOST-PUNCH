using UnityEngine;

/** Ghost charges up and releases a blast that pushes puncher away */
public class GA_Blast : GhostAction {

	Timer ti_blast;
	Timer ti_delay;

	public GA_Blast(Ghost g) : base(g) {
		ti_blast = new Timer(0, g.defaults.blast_data.CHARGE_TIME);
		ti_delay = new Timer(0, g.defaults.blast_data.POST_BLAST_WAIT);
	}

	public override void Enter() {
		ti_blast.Reset();
		ti_delay.Reset();

		ti_delay.Deactivate();

		ghost.ChangeAnimation("Blast");
}

	public override void Exit() {
	}

	public override void Update() {

		ti_blast.Tick(Time.deltaTime);
		if (ti_blast.FinishedThisFrame()) {
			GameObject orb = GameObject.Instantiate(ghost.defaults.blast_data.ORB);
			orb.transform.position = ghost.transform.position;
			ti_delay.Activate();
		}

		ti_delay.Tick(Time.deltaTime);
		if (ti_delay.FinishedThisFrame()) {
			ghost.ExitAction();
		}




	}
	
}
