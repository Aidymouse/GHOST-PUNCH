using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

class GA_POW_Slap : GhostAction {

	GameObject slash_effect_obj;
	VisualEffect slash_vfx;

	/** Gap between starting slap anim and processing the slap itself */
	Timer ti_slap_delay;
	/** Lenght of entire slap state */
	Timer ti_slap;

	GAD_Slap slap_data;

	public GA_POW_Slap(Ghost g) : base(g) {

		slap_data = ghost.defaults.slap_data;

		slash_effect_obj = Ghost.Instantiate(slap_data.SLASH_EFFECT_OBJECT, ghost.transform);
		slash_vfx = slash_effect_obj.GetComponent<VisualEffect>();
		ti_slap_delay = new Timer(slap_data.DELAY, slap_data.DELAY);
		ti_slap = new Timer(slap_data.TOTAL_SLAP_DURATION, slap_data.TOTAL_SLAP_DURATION);
	}

	public override void Enter() {

		ghost.nav_agent.destination = ghost.ghostPuncher.transform.position;

		ti_slap_delay.Reset();
		ti_slap.Reset();

		ghost.ChangeAnimation("Power_SlapLeft");

		ghost.nav_agent.isStopped = true;
	}


	public override void Update() {

		ti_slap_delay.Tick(Time.deltaTime);
		if (ti_slap_delay.FinishedThisFrame()) {
			float dist_to_gp = (this.ghost.transform.position - this.ghost.ghostPuncher.transform.position).magnitude;
			
			if (dist_to_gp <= slap_data.HIT_DISTANCE) {
				this.ghost.escape_meter += slap_data.ESCAPE_GAINED_ON_SLAP;
				this.ghost.ghostPuncher.GetComponent<GhostPuncher>().GetSlapped();
			}
			slash_vfx.Play();
		}

		ti_slap.Tick(Time.deltaTime);
		if (ti_slap.Finished()) {
			ghost.ExitAction();
		}

	}

	public override void Exit() {
			ghost.nav_agent.isStopped = false;
	}


}
