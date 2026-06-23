using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

	public Transform player_transform;
	InputAction action_look;

	public GPSettings settings;
	public float sensitivity;

	float lookX;
	float lookY;
	Vector2 lookDelta;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		action_look = InputSystem.actions.FindAction("Look");
		Cursor.lockState = CursorLockMode.Locked;
		sensitivity = settings.mouse_sensitivity;
	}

	// Update is called once per frame
	void Update()
	{
		// Looking Around
		lookDelta = action_look.ReadValue<Vector2>();
		Vector2 look_value = lookDelta;

		lookY -= look_value.y * sensitivity;
		lookY = Mathf.Clamp(lookY, -90f, 90f);

		lookX += look_value.x * sensitivity;

		Quaternion player_rot = player_transform.rotation;
		player_rot.eulerAngles = new Vector3(0, lookX, 0);
		player_transform.rotation = player_rot;

		Quaternion rot = transform.rotation;
		rot.eulerAngles = new Vector3(lookY, player_rot.eulerAngles.y, 0);
		transform.rotation = rot;

	}

	/* Because only the cameras up and down rotation is controlled, when the camera is set back to the player vCam, is looks back towards the HeadTarget. So we need to set the rotation of the player capsule if we want the movement from a different vcam to stay applied */
	public void SetRotation(float pitch) {
			lookX = pitch;
			this.transform.localRotation = Quaternion.Euler(lookX, 0f, 0f);
	}
}
