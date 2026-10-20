using UnityEngine;

public class S_Slowed : StatusEffect {

	public float SlowValue;

	public S_Slowed(float time, float slow_val) : base(StatusType.SLOWED, time) {
		SlowValue = slow_val;
	}

	public override float GetFloatValue(StatusAttribs attr) {
		if (attr == StatusAttribs.SLOWED_STRENGTH) {
			return SlowValue;
		}

		Debug.LogError("You asked me for a float value that I don't have! " + attr);
		return 0;
	}
	
	public override string GetDebugString() {
		return "["+Type+"] Dur: " + Duration.GetTime() + ", % Reduction: " + SlowValue;
	}
}

