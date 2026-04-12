using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "GhostPowerAttribs", menuName = "Scriptable Objects/GhostPowerAttribs")]
public class GhostPowerAttribs : ScriptableObject
{


	[Header("Power Debug Tools")]
	[Tooltip("Sets the default power index. Set to -1 to pick a random power. (0) Wave; (1) Slap; (2) Scream; ")]
	[Range(-1,2)]
	public int OVERRIDE_POWER_IDX;

    /** WAVE POWER **/
	[Header("Wave Power Attributes")]
    [Tooltip("Power that forces you back when you get hit with the wave")]
    public float WAVE_POWER;
    [Tooltip("How quickly the wave loses power when pushing the ghost puncher")]
    public float WAVE_DECAY;
    [Tooltip("The threshold at which push power is so weak the ghost puncher is no longer pushed")]
    public float WAVE_POWER_THRESHOLD;
    public float WAVE_CHARGE_TIME;
    public float WAVE_ACTIVE_DELAY_TIME;
    public float WAVE_ACTIVE_TIME;
    public float WAVE_HANG_TIME;

	[Header("Slap Power Attributes")]
    [Tooltip("(seconds)")]
    public float SLAP_CHARGE_TIME;
    [Tooltip("Distance from the ghost puncher to stop to do the slap")]
    public float SLAP_DISTANCE;
    [Tooltip("(seconds) time spent slapping + cooldown time")]
    public float SLAP_HANG_TIME;
    [Tooltip("(seconds) time after charge up before slap collision spawns")]
    public float SLAP_ACTIVE_DELAY_TIME;
    [Tooltip("(seconds) time the hitbox is active")]
    public float SLAP_ACTIVE_TIME;
    [Tooltip("Distance of ghost to be counter as a slap")]
    public float SLAP_HIT_DISTANCE;
		[Tooltip("The slash effect game object to use")]
		public GameObject SLASH_EFFECT_OBJECT;

	[Header("Scream Power Attributes")]
    public float SCREAM_CHARGE_TIME;
    public float SCREAM_HANG_TIME;
    public float SCREAM_ACTIVE_DELAY_TIME;
    public float SCREAM_ACTIVE_TIME;
    [Tooltip("Distance of ghost to be counter as a slap")]
    public float SCREAM_HIT_DISTANCE;
}
