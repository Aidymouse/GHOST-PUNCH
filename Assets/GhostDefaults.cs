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
    [Tooltip("Time spent ragdolled after being SMASHED!")]
    public float RAGDOLL_TIME;
    [Tooltip("Time spent recovering (getting up from ragdolling)")]
    public float RECOVERY_TIME;
    [Tooltip("Multiplier added to punch force when the ghost ragdolls")]
    public float MAKE_HER_FLY_FACTOR;

}
