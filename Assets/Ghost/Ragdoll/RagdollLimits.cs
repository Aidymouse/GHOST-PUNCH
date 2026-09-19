using UnityEngine;

[CreateAssetMenu(fileName = "RagdollLimits", menuName = "Scriptable Objects/RagdollLimits")]
public class RagdollLimits : ScriptableObject
{
	public Vector3 anchor;
	public Vector3 axis;
	public Vector3 secondary_axis;
	public ConfigurableJointMotion AngularXMotion;
	public ConfigurableJointMotion AngularYMotion;
	public ConfigurableJointMotion AngularZMotion;
	public float LowAngularXLimit;
	public float HighAngularXLimit;
	public float AngularYLimit;
	public float AngularZLimit;
}

