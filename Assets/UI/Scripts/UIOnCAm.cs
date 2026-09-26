using UnityEngine;

public class UIRotationDelay : MonoBehaviour
{
    [SerializeField] private float rotationSmoothSpeed = 5.0f;
    [Range(0f, 10f)]
    [SerializeField] private float maxAngleLimit = 2.0f;

    private Quaternion currentWorldRotation;

    private void Start()
    {
        currentWorldRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (transform.parent == null) return;

        Quaternion targetWorldRotation = transform.parent.rotation;

        currentWorldRotation = Quaternion.Slerp(
            currentWorldRotation, 
            targetWorldRotation, 
            rotationSmoothSpeed * Time.deltaTime
        );

        float angleDifference = Quaternion.Angle(currentWorldRotation, targetWorldRotation);
        
        if (angleDifference > maxAngleLimit)
        {
            float t = maxAngleLimit / angleDifference;
            currentWorldRotation = Quaternion.Slerp(targetWorldRotation, currentWorldRotation, t);
        }

        transform.rotation = currentWorldRotation;
    }
}
