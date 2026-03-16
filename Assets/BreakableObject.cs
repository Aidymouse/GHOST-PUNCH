using UnityEngine;


public class BreakableObject : MonoBehaviour
{

  public GameObject broken_obj;
  public float hp;
  /* When the broken object spawns, apply this rotation. */
  public Vector3 rotation_offset;
  // TODO: damage reduction modifier ?
  public ParticleSystem hit_particles;
  public ParticleSystem break_particles;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void GetPunched(float power, Vector3 punch_dir, RaycastHit hit) {


    if (hp > 0) {
      hp -= power;
      if (hp <= 0) {
	Instantiate(break_particles, hit.point, new Quaternion());
	Break(power, punch_dir);
      } else {
	// spawn particles
	// TODO: rotation
	if (hit_particles) {
	  Instantiate(hit_particles, hit.point, new Quaternion());
	}

	Rigidbody rb = this.GetComponent<Rigidbody>();
	if (rb) {
	  Vector3 blast_dir = punch_dir;

	  //crb.constraints = RigidbodyConstraints.None;
	  rb.isKinematic = false;
	  rb.AddForce(blast_dir.normalized * power);
	}
      }
    }


  }

  void Break(float power, Vector3 punch_dir) {
    Transform initRotation = this.transform;
    initRotation.Rotate(this.rotation_offset); // Local space ??

    if (broken_obj) {
      GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);
      //broken.SetActive(true);

      Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

      foreach (Rigidbody crb in rbs) {
	if (break_particles) {
	  Instantiate(break_particles, crb.transform.position, new Quaternion());
	}
	Vector3 blast_dir = punch_dir;

	//crb.constraints = RigidbodyConstraints.None;
	crb.isKinematic = false;
	crb.AddForce(blast_dir.normalized * power);
	//crb.gameObject.layer = LayerMask.NameToLayer("Punchable");
      }
    }

    Destroy(this.gameObject);

    /*
       Rigidbody[] hit_rbs = broken_obj.GetComponentsInChildren<Rigidbody>();

       foreach (Rigidbody crb in hit_rbs) {
       Vector3 blast_dir = punch_dir;

    //crb.constraints = RigidbodyConstraints.None;
    crb.isKinematic = false;
    crb.AddForce(blast_dir.normalized * 200);
    //crb.gameObject.layer = LayerMask.NameToLayer("Punchable");
    }
    */
  }
}
