using UnityEngine;

public class FlashController : MonoBehaviour {

	public Renderer renderer;

	Timer ti_flash;
	float flash_max;
	float flash_min;
	int num_flashes;


	void Awake() {
		ti_flash = new Timer(0, 0.25f);
		ti_flash.Deactivate();
	}

	/** Initiates flashing, set flashes to -1 for infinite. Optionally set other params too.
 	* TODO: color */
	public void StartFlashing(int flashes, float? flash_duration=null, float? min=null, float? max=null, Color? flash_color=null) {
		if (flash_duration is not null) {
			ti_flash.SetTime(flash_duration.Value, flash_duration.Value); 
			ti_flash.Reset();
		}

		if (flash_color is not null) {
			SetFlashColor(flash_color.Value);
		}

		if (min is not null) { flash_min = min.Value; }
		if (max is not null) { flash_max = max.Value; }

		num_flashes = flashes;
		ti_flash.Activate();
		//mesh_renderer.material.SetFloat("_FlashAmount", 0);
	}

	/** Handles flash timer logic to get the amount based on input */
	public void Update() {
		if (!ti_flash.IsActive()) {
			return;
		}

		ti_flash.Tick(Time.deltaTime);

		float half_period = ti_flash.default_time / 2;
		float flash_up = ti_flash.time_remaining - half_period;

		if (flash_up > 0) {
			float percent_flash_up = 1 - flash_up / half_period;
			SetFlashAmount(Lerp.Linear(flash_min, flash_max, percent_flash_up));
		} else {
			float percent_flash_down = ti_flash.time_remaining / half_period;
			SetFlashAmount(Lerp.Linear(flash_min, flash_max, percent_flash_down));
		}

		if (ti_flash.Finished()) {
			if (num_flashes > 0) {
				num_flashes -= 1;
				if (num_flashes <= 0) {
					StopFlashing();
					return;
				}
			}
			ti_flash.Reset();
		}

	}

	public void StopFlashing() {
		SetFlashAmount(0);
		ti_flash.Deactivate();
	}

	/** */
	public void Countdown(int flashes_remaining) {
		num_flashes = flashes_remaining;
	}
	

	/**  FN for setting the actual flash. For now it's just the first material on a renderer. */
	void SetFlashAmount(float amnt) {
			renderer.material.SetFloat("_FlashAmount", amnt);
	}

	void SetFlashColor(Color color) {
			renderer.material.SetColor("_Flash_Color", color);
	}


}
