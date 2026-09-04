using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Scream", menuName = "Scriptable Objects/GhostActionData/Scream")]
public class GAD_Scream : ScriptableObject
{
    public float SCREAM_CHARGE_TIME;
    public float SCREAM_HANG_TIME;
    public float SCREAM_ACTIVE_DELAY_TIME;
    public float SCREAM_ACTIVE_TIME;
    [Tooltip("Distance of ghost to be counter as a slap")]
    public float SCREAM_HIT_DISTANCE;
    [Tooltip("Percentage reduction in ghost punchers speed when screamed at")]
    public float SCREAM_SPEED_REDUCTION;
    [Tooltip("How long puncher is slowed for")]
    public float SCREAM_SPEED_REDUCTION_DURATION;
}
