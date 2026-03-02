using UnityEngine;

public class Timer
{
	public float time_remaining;
	public float default_time;
	public bool finished_this_frame;

	public Timer(float initial_time, float init_default_time=-1) {
		time_remaining = initial_time;
		default_time = init_default_time;
		finished_this_frame = false;
	}

	public void set(float new_time) {
		time_remaining = new_time;
	}

	public void reset() {
		time_remaining = default_time;
	}


	/** Assumed to be called every update frame, at the beginning of the frame */
	public float tick(float time) {
		finished_this_frame = false;

		if (!(time_remaining <= 0)) {
			time_remaining -= time;

			if (time_remaining <= 0) {
				finished_this_frame = true;
			}

		}

		return time_remaining;
	}

	public bool finished() {
		return time_remaining <= 0;
	}


}
