using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "GAD_Slap", menuName = "Scriptable Objects/GhostActionData/Slap")]
public class GAD_Slap : ScriptableObject
{
    [Tooltip("Distance from the ghost puncher to stop to do the slap")] public float STOP_DISTANCE;
    [Tooltip("(seconds) time after charge up before slap collision spawns")] public float DELAY;
    [Tooltip("Distance of ghost to be counter as a slap")] public float HIT_DISTANCE;
		[Tooltip("The slash effect game object to use")] public GameObject SLASH_EFFECT_OBJECT;
}
