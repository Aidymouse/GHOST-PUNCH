using UnityEngine;

[CreateAssetMenu(fileName = "GAD_StaggerLarge", menuName = "Scriptable Objects/GhostActionData/StaggerLarge")]
public class GAD_StaggerLarge : ScriptableObject
{
	[Tooltip("Time spent in stagger")]
	public float STAGGER_TIME;
	[Tooltip("Animation for large stagger")]
	public string ANIMATION;
	[Tooltip("Time taken to go from min flash to max flash. So basically this is the time of half a period")]
	public float FLASH_SPEED;
	[Tooltip("Lowest flash value")]
	public float FLASH_MIN;
	[Tooltip("Highest flash value")]
	public float FLASH_MAX;
}
