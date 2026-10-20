using UnityEngine;

/** Lets you queue target seeker commands that will play out. Commands play out after the previous target is attained. Basically just sets a new target, speed, etc. */
class TargetSeekerCommand {
	// Value to set the current to, if any
	float? current;
	// Desired target
	float? target;
	// Speed of this target seek
	float? speed;
	// Similar to speed
	float? time_taken;
}

public class TargetSeeker {
	
	float target;
	float current;
	float speed;

	public TargetSeeker(float cur, float targ, float speed) {
		current = current;
		target = targ;
	}

	public void SetTarget(float new_target) {
		target = new_target;
	}
	
	public void SetSpeed(float new_speed) { speed = new_speed; }

	public void SetCurrent(float current) {
	}

	public void Update() {
		float dist = Mathf.Abs(target - current);

		float delta = speed * Time.deltaTime;

		if (dist < delta) {
			target = current;
		}

		if (target > current) {
			current += delta;
		} else {
			current -= delta;
		}

	}

	/*
	public void GetToTargetIn(float target, float seconds) {
		const dist = Mathf.Abs();
	}
	*/
}
