using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostUI : MonoBehaviour
{
	public Ghost ghost;
	public GhostPuncher ghost_puncher;

	TMP_Text ui_escape_meter;
	TMP_Text ui_ectoplasm;
	TMP_Text txt_youlose;
	TMP_Text txt_fear_meter;

	Image hurt_indicator;
	Image slow_indicator;

	Timer ti_hurt_indicator;

	UIBar ghost_health_bar;
	UIBar poise_bar;
	UIBar escape_bar;
	UIBar stamina_bar;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
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

		TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

		foreach (TMP_Text text in texts) {
			switch (text.name) {
				case "EscapeMeter": 
					ui_escape_meter = text;
					break;

				case "Ectoplasm": 
					ui_ectoplasm = text;
					break;

				case "YouLose":
					txt_youlose = text;
					text.enabled = false;
					break;

				case "FearMeter":
					txt_fear_meter = text;
					break;

			}
		}

		UIBar[] bars = GetComponentsInChildren<UIBar>();
		foreach (UIBar bar in bars) {
			switch (bar.name) {
				case "EscapeBar":
					escape_bar = bar;
					escape_bar.SetMaxValue(ghost.escape_needed);
					break;
				case "PoiseBar":
					poise_bar = bar;
					//poise_bar.SetMaxValue(ghost.defaults.POISE);
					break;
				case "HealthBar":
					ghost_health_bar = bar;
					//ghost_health_bar.SetMaxValue(ghost.defaults.HP);
					break;
				case "StaminaBar":
					stamina_bar = bar;
					//stamina_bar.SetMaxValue(ghost_puncher.defaults.BASE_STAMINA);
					break;
			}
		}

		//escape_bar.SetValue(ghost.escape_meter);

		//ui_escape_meter = UnityEngine.GameObject.Find<TMP_Text>("EscapeMeter");
	}

	// Update is called once per frame
	void Update()
	{
		TickTimers();

		UpdateEscapeMeter(ghost.escape_meter);

		if (ghost.Escaped()) {
			EnableYouLose();
		}

		txt_fear_meter.SetText("Fear:\n" + Mathf.Floor(ghost.fear_meter));

		escape_bar.SetValue(ghost.escape_meter);
		ghost_health_bar.SetValue(ghost.hp);
		poise_bar.SetValue(ghost.poise);
		stamina_bar.SetValue(ghost_puncher.stamina);

		/** Hurt Indicator **/
		if (!ti_hurt_indicator.finished()) {
			Color hurt_color = hurt_indicator.color;
			hurt_color.a = ti_hurt_indicator.percent_complete();
			hurt_indicator.color = hurt_color;
		} 

		if (ti_hurt_indicator.finished_this_frame()) {
			Color hurt_color = hurt_indicator.color;
			hurt_color.a = 0.0f;
			hurt_indicator.color = hurt_color;
		}

		if (ghost_puncher.uiFlag_slapped_this_frame) {
			TriggerHurtIndicator();
			ghost_puncher.uiFlag_slapped_this_frame = false;
		}

		/** Slowed Indicator **/

	}

	void TickTimers() {
		ti_hurt_indicator.tick(Time.deltaTime);
	}

	public void UpdateEscapeMeter(float value) {
		ui_escape_meter.SetText("" + value);
	}

	public void UpdateEctoplasm(int plasm) {
		ui_ectoplasm.SetText("Ectoplasm: " + plasm);
	}

	public void TriggerHurtIndicator() {
		Color hurt_color = hurt_indicator.color;
		hurt_color.a = 0.6f;
		hurt_indicator.color = hurt_color;
		//hurt_indicator.color.a = 1;
		ti_hurt_indicator.reset();
	}

	public void EnableYouLose() {
		txt_youlose.enabled = true;
	}
}
