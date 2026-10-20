using UnityEngine;

[CreateAssetMenu(fileName = "PAD_Throw", menuName = "Scriptable Objects/PuncherAbilityData/Throw")]
public class PAD_Throw : ScriptableObject {
	[Tooltip("Force applied to thrown items")]
	public float THROW_FORCE;

	[Tooltip("Spin force applied to thrown items")]
	public float THROW_TORQUE;

	[Tooltip("(seconds)")]
	public float GRAB_DELAY;

	[Tooltip("(seconds)")]
	public float THROW_DELAY;
}
