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

  public void set(float new_time) {
    time_remaining = new_time;
  }

  public void reset() {
    time_remaining = default_time;
    fin_this_frame = false;
  }


  /** Assumed to be called every update frame, at the beginning of the frame */
  public float tick(float time) {
    if (!active) { return -1; }
    fin_this_frame = false;

    if (!(time_remaining <= 0)) {
      time_remaining -= time;

      if (time_remaining <= 0) {
	fin_this_frame = this.active && true;
      }

    }

    return time_remaining;
  }

  public bool finished() {
    return this.active && time_remaining <= 0;
  }

  public bool finished_this_frame() {
    return this.active && fin_this_frame;
  }

  public void activate() {
    this.active = true;
  }

  public void deactivate() {
    this.active = false;
  }



}
