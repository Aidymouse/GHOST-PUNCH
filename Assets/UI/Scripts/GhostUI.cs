using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyTextEffects;

public class GhostUI : MonoBehaviour
{
	public Ghost ghost;
	public GhostPuncher ghost_puncher;
	StaminaOrbs stamina_orbs;
	EscapeClock escape_clock;
	public GhostHealthBar ghost_health_bar;
	public GhostHealthBar ghost_poise_bar;
	public GhostHealthBar ghost_fear_bar;

	[Header("Text")]
	public TMP_Text txt_ectoplasm;
	public TMP_Text txt_fear_multiplier;
	public TextEffect txt_fear_jitter_effect;

	Image hurt_indicator;
	Image slow_indicator;

	Timer ti_hurt_indicator;

	[Tooltip("If true, we'll init as soon as we start. Should be false except when testing")]
	public bool init_on_start;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake() {
		stamina_orbs = GetComponentInChildren<StaminaOrbs>();
		escape_clock = GetComponentInChildren<EscapeClock>();
	}	

	void Start()
	{

		ti_hurt_indicator = new Timer(0.0f, 0.6f);

		Image[] images = GetComponentsInChildren<Image>();
		foreach (Image img in images) {
			switch (img.name) {
				case "HurtIndicator": {
					hurt_indicator = img;
					break;
				}
			}
		}


		//ui_escape_meter = UnityEngine.GameObject.Find<TMP_Text>("EscapeMeter");
		if (init_on_start) {
			InitUI(ghost, ghost_puncher);
		}
	}

	public void InitUI(Ghost ghost, GhostPuncher puncher) {
		ghost_health_bar.SetProportion(1);
		ghost_poise_bar.SetProportion(1);
	}

	// Update is called once per frame
	void Update()
	{
		TickTimers();

		ghost_health_bar.SetPropTarget(ghost.hp / ghost.defaults.HP);
		// TODO: while ghost is vulnerable, flash poise bar
		ghost_poise_bar.SetPropTarget(ghost.poise / ghost.max_poise);

		stamina_orbs.SetStamina(ghost_puncher.stamina);
		escape_clock.SetTimeLeft(1 - (ghost.escape_meter / ghost.escape_needed));

		/** Fear Bar **/
		if (ghost_puncher.fear_index == 0 && ghost_puncher.fear_meter == 0) {
			ghost_fear_bar.gameObject.SetActive(false);
		} else {
			ghost_fear_bar.gameObject.SetActive(true);

			// We need to find the percentage of the way to the next stage
			float fear_required_this_stage = ghost_puncher.defaults.FEAR_REQUIRED[ghost_puncher.fear_index+1];
			float threshold_low = ghost_puncher.fear_thresholds[ghost_puncher.fear_index];
			float fear_this_stage = ghost_puncher.fear_meter - threshold_low;
			float fear_stage_portion = fear_this_stage / fear_required_this_stage;
			float fear_portion = ghost_puncher.fear_index + fear_stage_portion;

			//Debug.Log("Fear threshold: [" + threshold_low + ", " + ghost_puncher.fear_thresholds[ghost_puncher.fear_index+1] + "], Fear Meter: " + ghost_puncher.fear_meter + ", Portion: " + fear_portion + ", Fear Requried This Stage: " + fear_required_this_stage);

			ghost_fear_bar.SetPropTarget(fear_portion);

			txt_fear_multiplier.SetText("x"+ghost_puncher.GetFearMultiplier());
			txt_fear_jitter_effect.Refresh();
		}

		/** Hurt Indicator **/
		if (!ti_hurt_indicator.Finished()) {
			Color hurt_color = hurt_indicator.color;
			hurt_color.a = ti_hurt_indicator.PercentComplete();
			hurt_indicator.color = hurt_color;
		} 

		if (ti_hurt_indicator.FinishedThisFrame()) {
			Color hurt_color = hurt_indicator.color;
			hurt_color.a = 0.0f;
			hurt_indicator.color = hurt_color;
		}

		if (ghost_puncher.uiFlag_slapped_this_frame) {
			TriggerHurtIndicator();
			ghost_puncher.uiFlag_slapped_this_frame = false;
		}

		/** TODO: Slowed Indicator **/

	}

	void TickTimers() {
		ti_hurt_indicator.Tick(Time.deltaTime);
	}


	public void UpdateEctoplasm(int plasm) {
		txt_ectoplasm.SetText(""+plasm);
	}

	public void TriggerHurtIndicator() {
		Color hurt_color = hurt_indicator.color;
		hurt_color.a = 0.6f;
		hurt_indicator.color = hurt_color;
		//hurt_indicator.color.a = 1;
		ti_hurt_indicator.Reset();
	}


	public void EndRun() {
	}
}
