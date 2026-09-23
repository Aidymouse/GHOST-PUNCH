using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "GhostSounds", menuName = "Scriptable Objects/Ghost/GhostSounds")]
public class GhostSounds : ScriptableObject
{
	[Tooltip("Sound the ghost makes when she is hit")]
	public AudioClip HIT_SCREAM;
	[Tooltip("Sound of the ghosts physical body getting hit by things")]
	public AudioClip HIT_SOUND;
	[Tooltip("Sound the ghost makes when she is ragdolled")]
	public AudioClip RAGDOLL_SCREAM;

}
