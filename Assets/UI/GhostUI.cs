using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostUI : MonoBehaviour
{
	public Ghost ghost;
	public GhostPuncher ghost_puncher;

	TMP_Text ui_escape_meter;
	TMP_Text ui_ectoplasm;

	Image hurt_indicator;

	Timer ti_hurt_indicator;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

		hurt_indicator = GetComponentInChildren<Image>();

		ti_hurt_indicator = new Timer(0.0f, 0.6f);

		foreach (TMP_Text text in texts) {
			switch (text.name) {
				case "EscapeMeter": {
										ui_escape_meter = text;
										break;
									}
				case "Ectoplasm": {
														ui_ectoplasm = text;
														break;
													}
			}
		}

		//ui_escape_meter = UnityEngine.GameObject.Find<TMP_Text>("EscapeMeter");
	}

	// Update is called once per frame
	void Update()
	{
		TickTimers();

		UpdateEscapeMeter(ghost.escape_meter);


		/** Hurt Indicator **/
		if (!ti_hurt_indicator.finished()) {
			Color hurt_color = hurt_indicator.color;
			hurt_color.a = ti_hurt_indicator.percent_complete();
			hurt_indicator.color = hurt_color;
		} 

		if (ghost_puncher.uiFlag_slapped_this_frame) {
			TriggerHurtIndicator();
			ghost_puncher.uiFlag_slapped_this_frame = false;
		}

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
		hurt_color.a = 6;
		hurt_indicator.color = hurt_color;
		//hurt_indicator.color.a = 1;
		ti_hurt_indicator.reset();
	}
}
