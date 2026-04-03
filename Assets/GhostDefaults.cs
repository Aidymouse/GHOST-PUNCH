using UnityEngine;

[CreateAssetMenu(fileName = "GhostDefaults", menuName = "Scriptable Objects/GhostDefaults")]
public class GhostDefaults : ScriptableObject
{

    [Tooltip("How fast (in degress/second I think) the ghost rotates towards you")]
    public float TURN_SPEED;
    [Tooltip("How long the ghost is stunned for")]
    public float HIT_STUN_TIME;
    [Tooltip("The poise value for the normal ghost. The ghost always reacts to hits in some way but if this breaks she stops moving and flinches.")]
    public float POISE;
    [Tooltip("If the ghost isn't hit for this much time, her poise replenishes to full")]
    public float POISE_RESTORE_TIMER;

}
