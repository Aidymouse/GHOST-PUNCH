using UnityEngine;

[CreateAssetMenu(fileName = "GhostDefaults", menuName = "Scriptable Objects/GhostDefaults")]
public class GhostDefaults : ScriptableObject
{

    [Tooltip("Amount of hit points")]
    public float HP;
    [Tooltip("How long the ghost is stunned for")]
    public float HIT_STUN_TIME;
    [Tooltip("The poise value for the normal ghost. The ghost always reacts to hits in some way but if this breaks she stops moving and flinches.")]
    public float POISE;
    [Tooltip("If the ghost isn't hit for this much time, her poise replenishes to full")]
    public float POISE_RESTORE_TIMER;
    [Tooltip("Time spent ragdolled after being SMASHED!")]
    public float RAGDOLL_TIME;
    [Tooltip("(radians) Upwards rotation added to punch direction when ghost is ragdolled")]
    public float RAGDOLL_ROTATION;
    [Tooltip("Time spent recovering (getting up from ragdolling)")]
    public float RECOVERY_TIME;
    [Tooltip("Multiplier added to punch force when the ghost ragdolls")]
    public float MAKE_HER_FLY_FACTOR;
		[Tooltip("How many seconds the ghost needs to charge to end the run")]
		public float ESCAPE_NEEDED;
		[Tooltip("How long the ghost spends vulnerable when knocked to 0 poise")]
		public float VULNERABLE_TIME;

		[Header("Ghost Shake")]
		[Tooltip("Duration of shake when ghost is punched")]
		public float PUNCHED_SHAKE_DURATION;
		[Tooltip("Strength of shake when ghost is punched")]
		public float PUNCHED_SHAKE_STRENGTH;

		[Header("Knockback")]
		[Tooltip("Time in knockback - should be very short!")]
		public float KNOCKBACK_TIMER;
		[Tooltip("Multiplier applied to punch force for knockback - should probably be below 1")]
		public float KNOCKBACK_FACTOR;


		[Header("Flashing")]
		public float PUNCHED_FLASH_PERIOD;
		public float PUNCHED_FLASH_MIN;
		public float PUNCHED_FLASH_MAX;
		public Color PUNCHED_FLASH_COLOR;


		[Header("Powers")]

		[Tooltip("(meters) range at which a mid-range power will be chosen")]
		public float MID_RANGE;
		[Tooltip("(meters) range beyond which a far-range power will be chosen")]
		public float FAR_RANGE;

		public GAD_Scream scream_data;
		public GAD_Slap slap_data;
		public GAD_Blast blast_data;
		public GAD_Scurry scurry_data;
		public GAD_Rush rush_data;
		public GAD_StaggerLarge stagger_large_data;
		public GAD_StaggerMedium stagger_medium_data;
}
