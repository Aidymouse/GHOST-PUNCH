using UnityEngine;

public class Status {
	public StatusTypes StatusType;
	public Status(StatusTypes s_type) {
		StatusType = s_type;
	}

	public virtual void Update() {}
	public virtual void Remove(GameObject subject) {}
	public virtual void Apply(GameObject subject) {}

}

public class TimedStatus : Status {
	public Timer Duration;

	public TimedStatus(StatusTypes s_type, float duration) : base(s_type) {
		Duration = new Timer(duration, duration);
	}


	public override void Update() {
		Duration.tick(Time.deltaTime);	
	}

}

public enum StatusTypes {
	SLOWED
}

public class Status_Slow : TimedStatus {
	public Status_Slow(float duration) : base(StatusTypes.SLOWED, duration) {}

	public override Apply(GameObject subject) {
		if (subject.GetComponent<GhostPuncher>()) {
			subject.GetComponent<GhostPuncher>()
		} else {
			Remove();
		}
	}




}

// STATUS       SLOT1
// Slowed       Multiplier
