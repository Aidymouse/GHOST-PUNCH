using UnityEngine;
using TMPro;

public class ShopTipJar : MonoBehaviour
{
	public GameObject particles_obj;
	public Transform particles_spawn;

	/** Timer that resets on click and keeps dollars flashing */
	public Timer ti_show_dollars;
	/** Timer that controls individual dollar flashes */
	public Timer ti_dollar_flash;
	/** Text that holds dollar signs */
	public TMP_Text dollar_text;
	/** Alternates to control dollar flashing */
	public int dollar_flash;

	public GameObject goop_obj;
	public Transform goop_spawn;
	
	public void Awake() {
		ti_show_dollars = new Timer(0, 2f);
		ti_show_dollars.Deactivate();

		ti_dollar_flash = new Timer(0, 0.2f);
		dollar_flash = 0;
	}

	public void Update() {
		if (ti_show_dollars.IsActive()) {

			ti_dollar_flash.Tick(Time.deltaTime);
			if (ti_dollar_flash.FinishedThisFrame()) {
				dollar_flash += 1;
				if (dollar_flash % 2 == 1) {
					dollar_text.SetText("");
				} else {
					dollar_text.SetText("$$$");
				}
				ti_dollar_flash.Reset();
			}

			ti_show_dollars.Tick(Time.deltaTime);
			if (ti_show_dollars.Finished()) {
				StopFlashingDollars();
			}

		}

	}

	public void StartFlashingDollars() {
		dollar_flash = 0;
		ti_dollar_flash.Reset();
		ti_dollar_flash.Activate();
		ti_show_dollars.Activate();
		dollar_text.SetText("$$$");
	}

	public void StopFlashingDollars() {
		ti_dollar_flash.Deactivate();
		ti_show_dollars.Deactivate();
		dollar_text.SetText("");
	}

	public void OnClick() {
		SpawnGoop();

		if (!ti_show_dollars.IsActive()) { StartFlashingDollars(); }
		ti_show_dollars.Reset();

	}


	public void SpawnGoop() {
		GameObject new_goop = Instantiate(goop_obj, this.transform);
		new_goop.transform.position = goop_spawn.position;

		GameObject new_particles = Instantiate(particles_obj);
		new_particles.transform.position = particles_spawn.position;
	}
	
}
