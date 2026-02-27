using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public Transform player_transform;

    InputAction action_look;

    public float sensitivity;

    float lookX;
    float lookY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	action_look = InputSystem.actions.FindAction("Look");

	Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
	// Looking Around
	Vector2 look_value = action_look.ReadValue<Vector2>();

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

}
