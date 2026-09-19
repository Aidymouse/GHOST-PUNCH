using UnityEngine;

[CreateAssetMenu(fileName = "GAD_StaggerLarge", menuName = "Scriptable Objects/GhostActionData/StaggerLarge")]
public class GAD_StaggerLarge : ScriptableObject
{
	[Tooltip("Time spent in stagger")] public float STAGGER_TIME;
}
