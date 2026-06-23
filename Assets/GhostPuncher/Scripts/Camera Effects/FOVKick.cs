using UnityEngine;

public class FOVKick : MonoBehaviour
{
    Camera cam;

    [Header("Base FOV")]
    public float baseFOV = 75f;

    [Header("Small Punch")]
    public float smallPunchFOV = 88f;
    public float smallKickSpeed = 14f;
    public float smallReturnSpeed = 6f;

    [Header("Big Punch")]
    public float bigPunchFOV = 100f;
    public float bigKickSpeed = 18f;
    public float bigReturnSpeed = 8f;

    [Header("Current State")]
    float targetFOV;
    float currentReturnSpeed;

    void Start()
    {
        cam = GetComponent<Camera>();
        baseFOV = cam.fieldOfView;
        targetFOV = baseFOV;
        currentReturnSpeed = smallReturnSpeed;
    }

    void Update()
    {
        // Smooth toward target
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * currentReturnSpeed
        );

        // Return logic
        if (Mathf.Abs(cam.fieldOfView - baseFOV) < 0.5f)
        {
            targetFOV = baseFOV;
        }
        else if (targetFOV > baseFOV)
        {
            targetFOV = Mathf.Lerp(
                targetFOV,
                baseFOV,
                Time.deltaTime * currentReturnSpeed
            );
        }
    }

    public void SmallKick()
    {
        targetFOV = smallPunchFOV;
        currentReturnSpeed = smallReturnSpeed;
    }

    public void BigKick()
    {
        targetFOV = bigPunchFOV;
        currentReturnSpeed = bigReturnSpeed;
    }
}