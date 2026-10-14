
using UnityEngine;

[CreateAssetMenu(fileName = "GAD_StaggerMedium", menuName = "Scriptable Objects/GhostActionData/StaggerMedium")]
public class GAD_StaggerMedium : ScriptableObject
{
	[Tooltip("Time spent in stagger")]
	public float STAGGER_TIME;
	[Tooltip("Animation for medium stagger")]
	public string ANIMATION;
}
