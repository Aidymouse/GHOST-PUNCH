using UnityEngine;

public enum StatusType {
	SLOWED
};

public enum StatusAttribs {
	SLOWED_STRENGTH=0
}

public class StatusEffect {
	public Timer Duration;
	public StatusType Type;

	public StatusEffect(StatusType type, float time) {
		Type = type;
		Duration = new Timer(time, time);
	}
	public virtual float GetFloatValue(StatusAttribs attr) { return 0; }

}

public class S_Slowed : StatusEffect {

	public float SlowValue;

	public S_Slowed(float time, float slow_val) : base(StatusType.SLOWED, time) {
		SlowValue = slow_val;
	}

	public override float GetFloatValue(StatusAttribs attr) {
		if (attr == StatusAttribs.SLOWED_STRENGTH) {
			return SlowValue;
		}

		Debug.Log("You asked me for a float value that I don't have! " + attr);
		return 0;
	}
	
}
