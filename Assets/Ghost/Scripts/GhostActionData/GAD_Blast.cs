
using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Blast", menuName = "Scriptable Objects/GhostActionData/Blast")]
public class GAD_Blast : ScriptableObject
{
	[Tooltip("Force that pushes you away")] public float POWER;
	[Tooltip("How quickly the wave loses power when pushing")] public float DECAY;
	[Tooltip("Threshold at which push power is so weak it no longer pushes ghost puncher back")] public float POWER_THRESHOLD;
    public float WAVE_CHARGE_TIME;
    public float WAVE_ACTIVE_DELAY_TIME;
    public float WAVE_ACTIVE_TIME;
    public float WAVE_HANG_TIME;
}
