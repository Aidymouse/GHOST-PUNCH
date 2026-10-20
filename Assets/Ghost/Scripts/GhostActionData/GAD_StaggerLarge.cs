using UnityEngine;

[CreateAssetMenu(fileName = "GAD_StaggerLarge", menuName = "Scriptable Objects/GhostActionData/StaggerLarge")]
public class GAD_StaggerLarge : ScriptableObject
{
	[Tooltip("Time spent in stagger")]
	public float STAGGER_TIME;
	[Tooltip("Animation for large stagger")]
	public string ANIMATION;
	[Tooltip("Time taken for 1 flash")]
	public float FLASH_PERIOD;
	[Tooltip("Lowest flash value")]
	public float FLASH_MIN;
	[Tooltip("Highest flash value")]
	public float FLASH_MAX;
	public Color FLASH_COLOR;
}
