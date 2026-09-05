using UnityEngine;

/* Configurable joint limits to be applied to the ghost */

[CreateAssetMenu(fileName = "GhostRagdollLimits", menuName = "Scriptable Objects/GhostRagdollLimits")]
public class GhostRagdollLimitConfig : ScriptableObject {
	public RagdollLimits spine;
	public RagdollLimits spine001;
	public RagdollLimits spine003;

	public RagdollLimits shoulderL;
	public RagdollLimits upperArmL;
	public RagdollLimits forearmL;
	public RagdollLimits handL;

	public RagdollLimits shoulderR;
	public RagdollLimits upperArmR;
	public RagdollLimits forearmR;
	public RagdollLimits handR;

	public RagdollLimits spine004;
	public RagdollLimits spine005;

	public RagdollLimits thighL;
	public RagdollLimits shinL;
	public RagdollLimits footL;

	public RagdollLimits thighR;
	public RagdollLimits shinR;
	public RagdollLimits footR;
}
