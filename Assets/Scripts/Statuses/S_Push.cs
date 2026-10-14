using UnityEngine;

public class S_Push : StatusEffect {
	public float push_power;
	public Vector3 dir;
	public float decay;

	public S_Push(float time, Vector3 d, float power, float dec) : base(StatusType.PUSH, time) 	{
		dir = d;
		push_power = power;
		decay = dec;
	}

	public override Vector3 GetVector3Value(StatusAttribs attr) {
		if (attr == StatusAttribs.PUSH_VECTOR) {
			return dir * push_power;
		}

		Debug.LogError("Attempting to get unknown vector3 in Push status: " + attr);
		return new Vector3(0, 0, 0);
	}

	public override void Update() {
		UpdateDuration();

		// TODO: There is probably a better way of making the push ease out
		if (push_power < 0.5) {
			push_power *= decay / 1.5f;
		} else {
			push_power *= decay;
		}

		if (push_power < 0.05) {
			remove_me = true;
		}
	}


}
