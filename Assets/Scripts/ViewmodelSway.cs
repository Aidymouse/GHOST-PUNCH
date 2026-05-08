using UnityEngine;

public class ViewmodelSway : MonoBehaviour
{
    public Transform cameraTransform;

    public float swayAmount = 3f;
    public float smoothSpeed = 8f;

    private Quaternion lastRotation;
    private Vector3 currentRotation;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastRotation = cameraTransform.rotation;
    }

    void LateUpdate()
    {
        Quaternion deltaRotation =
            cameraTransform.rotation * Quaternion.Inverse(lastRotation);

        Vector3 deltaEuler = deltaRotation.eulerAngles;

        deltaEuler.x = FixAngle(deltaEuler.x);
        deltaEuler.y = FixAngle(deltaEuler.y);

        Vector3 targetRotation = new Vector3(
            -deltaEuler.x * swayAmount,
            -deltaEuler.y * swayAmount,
            0
        );

        currentRotation = Vector3.Lerp(
            currentRotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(currentRotation);

        lastRotation = cameraTransform.rotation;
    }

    float FixAngle(float angle)
    {
        if (angle > 180)
            angle -= 360;

        return angle;
    }
}