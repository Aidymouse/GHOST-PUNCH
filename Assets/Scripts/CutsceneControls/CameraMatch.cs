using UnityEngine;

public class CutsceneCameraMatch : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;     // Player capsule (rotates Y)
    public Transform cameraRoot;     // Camera pivot (rotates X)
    public Transform mainCamera;     // Main Camera (with CinemachineBrain)

    [Header("Optional")]
    public FPSLook fpsLook;          // Reference to your look script

    public void MatchCamera()
    {
        if (mainCamera == null) return;

        // Get the direction the camera is facing
        Vector3 forward = mainCamera.forward;

        // --- YAW (left/right) ---
        Vector3 flatForward = new Vector3(forward.x, 0f, forward.z);

        if (flatForward.sqrMagnitude > 0.001f)
        {
            playerBody.forward = flatForward;
        }

        // --- PITCH (up/down) ---
        float pitch = Mathf.Asin(forward.y) * Mathf.Rad2Deg;

        cameraRoot.localRotation = Quaternion.Euler(-pitch, 0f, 0f);

        // --- IMPORTANT: sync with FPSLook so it doesn't snap back ---
        if (fpsLook != null)
        {
            fpsLook.SetRotation(-pitch);
        }
    }
}