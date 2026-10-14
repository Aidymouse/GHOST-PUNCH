
using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Rush", menuName = "Scriptable Objects/GhostActionData/Rush")]
public class GAD_Rush : ScriptableObject
{
	[Tooltip("Range at which ghost is close enough to stop charging")]
	public float CLOSE_ENOUGH;
	[Tooltip("Speed ghost turns while rushing")]
	public float TURN_SPEED;
}
