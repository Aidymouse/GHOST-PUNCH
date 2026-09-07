

using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Twitch", menuName = "Scriptable Objects/GhostActionData/Twitch")]
public class GAD_Twitch : ScriptableObject
{
	[Tooltip("Minimum time of twitch")] public int MIN_DURATION;
	[Tooltip("Max time of twitch")] public int MAX_DURATION;
}
