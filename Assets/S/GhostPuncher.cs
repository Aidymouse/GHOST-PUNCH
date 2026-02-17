using UnityEngine;
using UnityEngine.InputSystem;


public class GhostPuncher : MonoBehaviour
{
    InputAction action_attack;
    InputAction action_move;

    public GhostUI ui;

    float move_speed;

    int ectoplasm = 0;

    LayerMask layer_punchable;

    const float PUNCH_RANGE = 2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	action_attack = InputSystem.actions.FindAction("Attack");
	action_move = InputSystem.actions.FindAction("Move");

	move_speed = 5;

	layer_punchable = LayerMask.GetMask("Punchable");
        
    }

    // Update is called once per frame
    void Update()
    {
	// Attacking
	if (action_attack.WasPerformedThisFrame()) {
	  punchControls();
	}

	// Moving
	moveControls();
        
    }


    void punchControls() {

		RaycastHit attack_hit;

		Camera cam = this.GetComponentInChildren<Camera>();

		Animator arm_animator = this.GetComponentInChildren<Animator>();

		arm_animator.SetTrigger("DoPunch");

		//Vector3 ray_dir = transform.TransformDirection(Vector3.forward);
		Vector3 ray_dir = cam.transform.TransformDirection(Vector3.forward);

		if (Physics.Raycast(cam.transform.position, ray_dir, out attack_hit, PUNCH_RANGE, layer_punchable)) {
			Debug.DrawRay(transform.position, ray_dir, Color.red, 1, false);

			Rigidbody hit_rb = attack_hit.rigidbody;
			Collider hit_col = attack_hit.collider;


			if (hit_rb != null) {
			  // The object should really be taking care of this
				//hit_rb.AddForce(transform.TransformDirection(Vector3.forward) * 1000);
			}

			if (hit_col == null) {
			  return;
			}

			if (hit_col.CompareTag("Breakable")) {

				Destroy(hit_col);

				Rigidbody[] hit_rbs = hit_col.GetComponentsInChildren<Rigidbody>();

				foreach (Rigidbody crb in hit_rbs) {
					Vector3 blast_dir = ray_dir;
					blast_dir.x += Random.Range(-3, 3);
					blast_dir.y += Random.Range(-3, 3);
					blast_dir.z += Random.Range(-3, 3);
					crb.constraints = RigidbodyConstraints.None;
					crb.AddForce(blast_dir.normalized * 1000);
					//crb.gameObject.layer = LayerMask.NameToLayer("Punchable");
				}
			} else if (hit_col.CompareTag("Ghost")) {
			  Ghost g = hit_col.gameObject.GetComponent<Ghost>();
			  g.GetPunched();
			  ectoplasm += 5;
			  ui.UpdateEctoplasm(ectoplasm);
			}



		}
	
    }

  void moveControls() {
    Vector2 move_value = action_move.ReadValue<Vector2>();

    Vector3 movement = transform.TransformDirection(Vector3.forward);
    movement.y = 0;
    movement = movement.normalized;

    Vector3 movement_hor = transform.TransformDirection(Vector3.right);
    movement_hor.y = 0;
    movement_hor = movement_hor.normalized;

    if (move_value.x > 0) {
      // Move right
      transform.Translate(movement_hor * move_speed * Time.deltaTime, Space.World);
    } else if (move_value.x < 0) {
      // Move left
      transform.Translate(movement_hor * -move_speed * Time.deltaTime, Space.World);
    }

    if (move_value.y > 0) {
      transform.Translate(movement * move_speed * Time.deltaTime, Space.World);
    } else if (move_value.y < 0) {
      transform.Translate(movement * -move_speed * Time.deltaTime, Space.World);
    }
  }
}
