using UnityEngine;

public class Timer
{
  public float time_remaining;
  public float default_time;
  private bool fin_this_frame;
  private bool active;

  public Timer(float initial_time, float init_default_time=-1) {
    time_remaining = initial_time;
    default_time = init_default_time;
    fin_this_frame = false;

    active = true;
  }

  public void Set(float new_time) {
    time_remaining = new_time;
  }

	public void SetTime(float new_time, float? max_time=null) {
		this.time_remaining = new_time;
		if (max_time is not null) {
			this.default_time = (float)max_time;
		}
	}

  public void Reset() {
    time_remaining = default_time;
    fin_this_frame = false;
  }
	
	/* Gets the percentage of the way through the timer. E.g. 0.0 = just started, 1.0 = ended. Returns -1 if the timer doesn't have a default time set */
	public float GetPercentage() {
		if (default_time == -1) { return -1.0f; }
		float percentage = 1.0f - (time_remaining / default_time);
		
		return percentage;
	}

	public float PercentComplete() {
		return this.time_remaining / this.default_time;
	}


  /** Assumed to be called every update frame, at the beginning of the frame */
  public float Tick(float time) {
    if (!active) { return -1; }
    fin_this_frame = false;

    if (!(time_remaining <= 0)) {
      time_remaining -= time;

      if (time_remaining <= 0) {
				time_remaining = 0;
				fin_this_frame = this.active && true;
      }

    }

    return time_remaining;
  }

  public bool Finished() {
    return this.active && time_remaining <= 0;
  }


  public bool FinishedThisFrame() {
    return this.active && fin_this_frame;
  }

  public void Activate() {
    this.active = true;
  }

  public void Deactivate() {
    this.active = false;
  }

  public bool IsActive() {
    return this.active;
  }
}
