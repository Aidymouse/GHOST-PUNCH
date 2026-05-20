using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDecay = 5f;
    public float maxShakeIntensity = 0.25f;

    Vector3 originalPos;
    float shakeIntensity;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (shakeIntensity > 0)
        {
            Vector3 offset;
            {
                offset = Random.insideUnitSphere;
            }

            transform.localPosition = originalPos + offset * shakeIntensity;

            shakeIntensity -= Time.deltaTime * shakeDecay;
        }
        else
        {
            shakeIntensity = 0f;
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float intensity)
    {
        shakeIntensity += intensity;

        // clamp so punches don’t explode the camera
        shakeIntensity = Mathf.Min(shakeIntensity, maxShakeIntensity);
    }
}