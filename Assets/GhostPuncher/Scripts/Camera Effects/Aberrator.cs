using UnityEngine;
using UnityEngine.Rendering;

/** Controls aberration volume */
public class Aberrator : MonoBehaviour {
	public Volume aberration_volume;
	Timer ti_up;
	Timer ti_down;
	public float max_aberration;

	void Awake() {
		ti_up = new Timer(0);
		ti_up.Deactivate();
		ti_down = new Timer(0);
		ti_down.Deactivate();
	}

	void Update() {
		if (ti_up.IsActive()) {
			if (ti_up.Finished()) {
				ti_up.Deactivate();
				ti_down.Activate();
			}

			ti_up.Tick(Time.deltaTime);
			aberration_volume.weight = ti_up.LerpValue(0, max_aberration, true);
		}

		if (ti_down.IsActive()) {
			ti_down.Tick(Time.deltaTime);
			aberration_volume.weight = ti_down.LerpValue(0, max_aberration);
			if (ti_down.Finished()) {
				ti_down.Deactivate();
			}
		}
	}

	public void StartAberrating(float max, float dur_up, float dur_down) {
		max_aberration = max;
		ti_up.SetTime(dur_up, dur_up);
		ti_down.SetTime(dur_down, dur_down);
		ti_up.Activate();
	}

	public string GetDebugString() {
		string state = "";
		state += "Max: "+max_aberration + "\n";
		state += "Volume Weight: "+aberration_volume.weight + "\n";
		// Can be null when ghost UI isn't on yet
		if (ti_up is not null) {
			state += "Up: "+ti_up.GetDebugString() + "\n";
		} else {
			state += "Up: Not initialized";
		}
		if (ti_down is not null) {
			state += "Down: "+ti_down.GetDebugString() + "\n";
		} else {
			state += "Down: Not initialized";
		}
		return state;
	}


}
