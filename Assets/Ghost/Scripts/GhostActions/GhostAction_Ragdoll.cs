
using UnityEngine;

public class GhostAction_Ragdoll : GhostAction {

	public GhostAction_Ragdoll(Ghost g) : base(g) {}

	public override void Enter() {

		Debug.Log("Entering Ragdoll State");
		ghost.ti_ragdoll.reset();

		/*
		ghost.DisableAnimator();
		ghost.EnableRagdoll();
		*/
		ghost.nav_agent.isStopped = true;
		ghost.vulnerable = false;
		ghost.ragdoll_animator.MasterAlpha = 0;
	}

	public override void Exit() {
		Vector3 ragdoll_offset = ghost.transform.position - ghost.rig_core.transform.position;
		ragdoll_offset.y = 0;
		// TODO: this causes the ragdoll to go flying, but i need *something* like this
		//ghost.transform.position -= ragdoll_offset;
		//ghost.rig_core.position += ragdoll_offset;
		//Debug.Log(ragdoll_offset);

		ghost.DisableRagdoll();
		ghost.EnableAnimator();
	}

	public override void Update() {
		Debug.Log("Ragdoll Update");
		ghost.ti_ragdoll.tick(Time.deltaTime);

		// TODO: somewhere nearing the right track, but maybe too much
		// I need some way of telling the ragdoll to teleport to a new position...
		Vector3 ragdoll_offset = ghost.transform.position - ghost.rig_core.transform.position;
		ragdoll_offset.y = 0;
		ghost.transform.position -= ragdoll_offset;

		if (ghost.ti_ragdoll.finished_this_frame()) {
			ghost.ti_recovery.set(1);
			ghost.EnterAction(GhostActions.GET_UP);
		}
	}

}
