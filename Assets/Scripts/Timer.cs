using UnityEngine;

public enum LerpTypes {
	EASE_IN,
	EASE_OUT,
	LINEAR
}

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
		if (this.default_time == 0) { return 1; }
		return this.time_remaining / this.default_time;
	}

	/**
 	* Lerps between timer start and end. 1.0 at start, 0.0 at timer end.
 	* @param lerp_type - Used to change type
 	* @param backwards - Makes it 0 at start, 1.0 at end
 	*/
	public float GetLerped(LerpTypes lerp_type=LerpTypes.LINEAR, bool backwards=false) {
		// TODO:
		if (lerp_type == LerpTypes.EASE_OUT) {
			return Lerp.EaseOutCubic(PercentComplete());
		} else if (lerp_type == LerpTypes.EASE_IN) {
			return Lerp.EaseInCubic(PercentComplete());
		}

		return PercentComplete();

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
