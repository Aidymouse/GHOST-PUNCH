using UnityEngine;

public enum StatusType {
	SLOWED,
	PUSH,
};

public enum StatusAttribs {
	SLOWED_STRENGTH=0,
	PUSH_VECTOR=1
}

/** Pretttty bad wrapper class for status effects.
 * Status effects don't apply to entities directly, they just provide data and let entities implment them.
 * */
public class StatusEffect {
	public Timer Duration;
	public StatusType Type;
	public bool remove_me;

	public StatusEffect(StatusType type, float time) {
		Type = type;
		Duration = new Timer(time, time);
		remove_me = false;
	}

	public virtual float GetFloatValue(StatusAttribs attr) { 
		Debug.LogError("Attempting to get Float from status that has not implemented GetFloatValue");
return 0; 
	}

	public virtual Vector3 GetVector3Value(StatusAttribs attr) { 
		Debug.LogError("Attempting to get Vector3 from status that has not implemented GetVector3Value");
		return new Vector3(0, 0, 0);
	}

	public virtual void Update() {
		UpdateDuration();
	}

	public void UpdateDuration() {
		Duration.Tick(Time.deltaTime);
		if (Duration.FinishedThisFrame()) {
			remove_me = true;
		}
	}

	public virtual string GetDebugString() {
		return "["+Type+"] No debug string available";
	}

}

