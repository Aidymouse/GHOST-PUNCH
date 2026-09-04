using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "GAD_Slap", menuName = "Scriptable Objects/GhostActionData/Slap")]
public class GAD_Slap : ScriptableObject
{
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
}
