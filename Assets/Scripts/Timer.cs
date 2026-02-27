using UnityEngine;

public class Timer
{
	public float time_remaining;
	public float default_time;
	public bool active;

	public Timer(float initial_time, float init_default_time=-1, bool init_active=true) {
		time_remaining = initial_time;
		default_time = init_default_time;
		active = init_active;
	}



	public void set(float new_time) {
		time_remaining = new_time;
	}

	public void reset() {
		time_remaining = default_time;
	}

	public void pause() {
		active = false;
	}

	public void start() {
		active = true;
	}

	public float tick(float time) {
		if (!active) return time_remaining;
		time_remaining -= time;
		return time_remaining;
	}    


	public bool finished() {
		return time_remaining <= 0;
	}
}
