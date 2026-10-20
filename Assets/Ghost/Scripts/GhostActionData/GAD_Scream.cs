using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Scream", menuName = "Scriptable Objects/GhostActionData/Scream")]
public class GAD_Scream : ScriptableObject
{
    [Tooltip("Distance of ghost to be counter as a slap")]
    public float RANGE;
    [Tooltip("Percentage reduction in ghost punchers speed when screamed at")]
    public float SPEED_REDUCTION;
    [Tooltip("How long puncher is slowed for")]
    public float SPEED_REDUCTION_DURATION;
		[Tooltip("Delay from anim start to scream happening")]
    public float DELAY;
		[Tooltip("Time spent hanging around after scream animation finishes")]
    public float HANG_TIME;
		public AudioClip SCREAM_SFX;
}
