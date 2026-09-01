using UnityEngine;

public class KickoutEffectController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float effectStrength = 0f;

    private static readonly int EffectStrengthID =
        Shader.PropertyToID("_EffectStrength");

    private void Update()
    {
        Shader.SetGlobalFloat(EffectStrengthID, effectStrength);
    }
}