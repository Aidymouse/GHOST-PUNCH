//thank you chatgpt, sorry Aidan :^( please fix

using UnityEngine;
using FMODUnity;

public class CameraAngleFMOD : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public StudioEventEmitter emitter;

    [Header("FMOD")]
    public string parameterName = "YourParameter";
    public float fadeSpeed = 3f;

    private float currentValue = -1f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null || emitter == null)
            return;

        // Current Y rotation (0-360)
        float yRotation = targetCamera.transform.eulerAngles.y;

        // Determine which angle is closer
        float distanceTo0 = Mathf.Abs(Mathf.DeltaAngle(yRotation, 0f));
        float distanceTo90 = Mathf.Abs(Mathf.DeltaAngle(yRotation, -90f));

        float targetValue = (distanceTo0 < distanceTo90) ? 0f : 1f;

        // Smoothly fade toward the target value
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * fadeSpeed);

        // Send the parameter to FMOD
        emitter.SetParameter(parameterName, currentValue);
    }
}