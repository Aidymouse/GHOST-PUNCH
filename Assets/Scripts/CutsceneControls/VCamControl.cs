using UnityEngine;
using UnityEngine.InputSystem;

public class FPSLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;   // Y rotation
    public Transform cameraRoot;   // X rotation
    public GhostPuncher player;    // for cutscene state

    [Header("Settings")]
    public float sensitivity = 0.1f;
    public float maxLookAngle = 85f;

    InputAction look;

    float xRotation;

    void Start()
    {
        look = InputSystem.actions.FindAction("Look");

        LockCursor();
    }

    void Update()
    {
        // Handle cursor relocking on click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            LockCursor();
        }

        // Unlock cursor with Escape
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        // STOP CAMERA CONTROL DURING CUTSCENE
        if (player != null && player.inCutscene)
            return;

        if (look == null) return;

        Vector2 input = look.ReadValue<Vector2>();

        float mouseX = input.x * sensitivity;
        float mouseY = input.y * sensitivity;

        // --- YAW (left/right) ---
        playerBody.Rotate(Vector3.up * mouseX);

        // --- PITCH (up/down) ---
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Called by CutsceneCameraMatch
    public void SetRotation(float pitch)
    {
        xRotation = pitch;
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}