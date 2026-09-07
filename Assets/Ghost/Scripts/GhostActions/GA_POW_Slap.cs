using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

enum GhostSlapPhase {
	CLOSING_DISTANCE,
	SLAPPING
}

class GA_POW_Slap : GhostAction {

	NavMeshAgent nav_agent;
	GameObject slash_effect_obj;
	VisualEffect slash_vfx;

	GhostSlapPhase phase;
	Timer ti_slap_delay;
	GAD_Slap slap_data;

	public GA_POW_Slap(Ghost g) : base(g) {

		nav_agent = ghost.get_nav_agent();

		slap_data = ghost.defaults.slap_data;

		slash_effect_obj = Ghost.Instantiate(slap_data.SLASH_EFFECT_OBJECT, ghost.transform);
		slash_vfx = slash_effect_obj.GetComponent<VisualEffect>();
		ti_slap_delay = new Timer(slap_data.DELAY, slap_data.DELAY);
	}

	public override void Enter() {
		NavMeshAgent nav_agent = this.ghost.get_nav_agent();

		Vector3 ghostPuncher_position = ghost.ghostPuncher.transform.position;
		nav_agent.destination = ghost.ghostPuncher.transform.position;
		nav_agent.stoppingDistance = slap_data.STOP_DISTANCE;
		
		phase = GhostSlapPhase.CLOSING_DISTANCE;
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
		nav_agent.destination = ghost.ghostPuncher.transform.position;

		float dist_to_puncher = (ghost.ghostPuncher.transform.position - ghost.transform.position).magnitude;
		if (dist_to_puncher < slap_data.STOP_DISTANCE) {
			StartSlap();
		}
	
	}

	void StartSlap() {
			ti_slap_delay.Reset();
			ghost.ChangeAnimation("Power_SlapLeft");
			phase = GhostSlapPhase.SLAPPING;
			nav_agent.isStopped = true;
	}

	void Update_Slapping() {
		ti_slap_delay.Tick(Time.deltaTime);

		if (ti_slap_delay.Finished()) {
			float dist_to_gp = (this.ghost.transform.position - this.ghost.ghostPuncher.transform.position).magnitude;
			Debug.Log("Slap Dist: "+dist_to_gp);
			if (dist_to_gp <= slap_data.HIT_DISTANCE) {
				this.ghost.escape_meter += 10;
				this.ghost.ghostPuncher.GetComponent<GhostPuncher>().GetSlapped();
			}
			slash_vfx.Play();
			ghost.ExitAction();
		}

	}

	public override void Exit() {
			nav_agent.isStopped = false;
			nav_agent.stoppingDistance = 0;
	}


}
