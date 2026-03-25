using UnityEngine;

[CreateAssetMenu(fileName = "GhostPowerAttribs", menuName = "Scriptable Objects/GhostPowerAttribs")]
public class GhostPowerAttribs : ScriptableObject
{


    /** WAVE POWER **/
    [Tooltip("(seconds) Time the ghost spends charging up the shockwave")]
    public float WAVE_CHARGE_TIME;
    [Tooltip("Power that forces you back when you get hit with the wave")]
    public float WAVE_POWER;
    [Tooltip("How quickly the wave loses power when pushing the ghost puncher")]
    public float WAVE_DECAY;
    [Tooltip("The threshold at which push power is so weak the ghost puncher is no longer pushed")]
    public float WAVE_POWER_THRESHOLD;
    // [Tooltip("Poise to break the wave power")]
    // public float WAVE_POISE;
    [Tooltip("(seconds) How long the ghost pauses after releasing a wave")]
    public float WAVE_HANG_TIME;
    // [Tooltip("How fast the wave travels")]
    // public float WAVE_SPEED;
    [Tooltip("(seconds)")]
    public float SLAP_CHARGE_TIME;
    [Tooltip("Distance from the ghost puncher to stop to do the slap")]
    public float SLAP_DISTANCE;
    [Tooltip("(seconds) time spent slapping + cooldown time")]
    public float SLAP_HANG_TIME;
    [Tooltip("(seconds) time after charge up before slap collision spawns")]
    public float SLAP_DO_TIME;
}
