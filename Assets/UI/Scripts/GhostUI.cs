using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostUI : MonoBehaviour
{
	public Ghost ghost;
	public GhostPuncher ghost_puncher;
	StaminaOrbs stamina_orbs;
	EscapeClock escape_clock;
	public GhostHealthBar ghost_health_bar;
	public GhostHealthBar ghost_poise_bar;

	[Header("Text")]
	public TMP_Text txt_ectoplasm;
	public TMP_Text txt_fear_multiplier;

	Image hurt_indicator;
	Image slow_indicator;

	Timer ti_hurt_indicator;

	[Header("Fear UI")]
	public UIBar fear_bar;
	public UIBar fear_reset_bar;

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
		fear_bar.SetValue(0);
		fear_reset_bar.SetValue(0);
	}

	// Update is called once per frame
	void Update()
	{
		TickTimers();

		ghost_health_bar.SetPropTarget(ghost.hp / ghost.defaults.HP);
		// TODO: while ghost is vulnerable, flash poise bar
		ghost_poise_bar.SetPropTarget(ghost.poise / ghost.max_poise);

		stamina_orbs.SetStamina(ghost_puncher.stamina);
		escape_clock.SetTimeLeft(ghost.escape_meter / ghost.escape_needed);

		/** Fear Bar **/
		// The goal for the fear bar changes based on punchers current multiplier
		float fear_required = ghost_puncher.GetFearRequired();
		if (fear_required > 0) {
			float fear_meter = ghost_puncher.fear_meter;
			float fear_portion = fear_meter / fear_required;
			fear_bar.SetValue(fear_portion);
		} else {
			fear_bar.SetValue(1);
		}

		fear_reset_bar.SetValue(ghost_puncher.ti_fear_reset.PercentComplete());

		txt_fear_multiplier.SetText("x"+ghost_puncher.GetFearMultiplier());

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
