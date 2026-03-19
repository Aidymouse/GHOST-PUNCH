using UnityEngine;

[CreateAssetMenu(fileName = "GhostDefaults", menuName = "Scriptable Objects/GhostDefaults")]
public class GhostDefaults : ScriptableObject
{

    [Tooltip("If this much time passes between hits, hit stun resistance is reset to 0")]
    public float HIT_STUN_RESET_TIME;
    [Tooltip("How long the ghost is stunned for")]
    public float HIT_STUN_TIME;
    [Tooltip("The value that hit stun resistance goes up by when the ghost gets hit")]
    public float HIT_STUN_RESISTANCE_GAIN;
    [Tooltip("The poise value for the normal ghost. The ghost always reacts to hits in some way but if this breaks she stops moving and flinches.")]
    public float DEFAULT_POISE;

    /** WAVE POWER **/
    [Tooltip("(seconds) Time the ghost spends charging up the shockwave")]
    public float WAVE_CHARGE_TIME;
    [Tooltip("Power that forces you back when you get hit with the wave")]
    public float WAVE_POWER;
    [Tooltip("Poise to break the wave power")]
    public float WAVE_POISE;
    [Tooltip("(seconds) How long the ghost pauses after releasing a wave")]
    public float WAVE_HANG_TIME;
    [Tooltip("How fast the wave travels")]
    public float WAVE_SPEED;
}
