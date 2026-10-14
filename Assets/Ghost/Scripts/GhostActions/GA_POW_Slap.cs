using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

enum GhostSlapPhase {
	CLOSING_DISTANCE,
	SLAPPING
}

class GA_POW_Slap : GhostAction {

	GameObject slash_effect_obj;
	VisualEffect slash_vfx;

	GhostSlapPhase phase;
	Timer ti_slap_delay;
	GAD_Slap slap_data;

	public GA_POW_Slap(Ghost g) : base(g) {

		slap_data = ghost.defaults.slap_data;

		slash_effect_obj = Ghost.Instantiate(slap_data.SLASH_EFFECT_OBJECT, ghost.transform);
		slash_vfx = slash_effect_obj.GetComponent<VisualEffect>();
		ti_slap_delay = new Timer(slap_data.DELAY, slap_data.DELAY);
	}

	public override void Enter() {

		Vector3 ghostPuncher_position = ghost.ghostPuncher.transform.position;
		ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;
		//ghost.nav_agent.stoppingDistance = slap_data.STOP_DISTANCE;
		
		// TODO: come back and remove all this closing distance crap
		phase = GhostSlapPhase.SLAPPING;
		StartSlap();
	}


	public override void Update() {
		if (phase == GhostSlapPhase.CLOSING_DISTANCE) {
			Update_ClosingDistance();
		} else if (phase == GhostSlapPhase.SLAPPING) {
			Update_Slapping();
		}
	}

	/** Sub-states **/
	void Update_ClosingDistance() {
		ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;

		// TODO: AND she's facing the right way!
		if (ghost.nav_agent.remainingDistance < slap_data.STOP_DISTANCE) {
			StartSlap();
		}
	
	}

	void StartSlap() {
			ti_slap_delay.Reset();
			ghost.ChangeAnimation("Power_SlapLeft");
			phase = GhostSlapPhase.SLAPPING;
			ghost.nav_agent.isStopped = true;
	}

	void Update_Slapping() {
		ti_slap_delay.Tick(Time.deltaTime);

		if (ti_slap_delay.Finished()) {
			float dist_to_gp = (this.ghost.transform.position - this.ghost.ghostPuncher.transform.position).magnitude;
			//Debug.Log("Slap Dist: "+dist_to_gp);
			if (dist_to_gp <= slap_data.HIT_DISTANCE) {
				this.ghost.escape_meter += slap_data.ESCAPE_GAINED_ON_SLAP;
				this.ghost.ghostPuncher.GetComponent<GhostPuncher>().GetSlapped();
			}
			slash_vfx.Play();
			ghost.ExitAction();
		}

	}

	public override void Exit() {
			ghost.nav_agent.isStopped = false;
	}


}
