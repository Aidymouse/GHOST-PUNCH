using UnityEngine;

[CreateAssetMenu(fileName = "GhostDefaults", menuName = "Scriptable Objects/GhostDefaults")]
public class GhostDefaults : ScriptableObject
{

    [Tooltip("How fast (in degress/second I think) the ghost rotates towards you")]
    public float TURN_SPEED;
    [Tooltip("If this much time passes between hits, hit stun resistance is reset to 0")]
    public float HIT_STUN_RESET_TIME;
    [Tooltip("How long the ghost is stunned for")]
    public float HIT_STUN_TIME;
    [Tooltip("The value that hit stun resistance goes up by when the ghost gets hit")]
    public float HIT_STUN_RESISTANCE_GAIN;
    [Tooltip("The poise value for the normal ghost. The ghost always reacts to hits in some way but if this breaks she stops moving and flinches.")]
    public float DEFAULT_POISE;

}
