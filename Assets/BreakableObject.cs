using UnityEngine;


public class BreakableObject : MonoBehaviour
{

  public GameObject broken_obj;
  public float hp;
  /* When the broken object spawns, apply this rotation. */
  public Vector3 rotation_offset;
  // TODO: damage reduction modifier ?


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void GetPunched(float power, Vector3 punch_dir) {

    if (hp > 0) {
      hp -= power;
      if (hp <= 0) {
	Break(power, punch_dir);
      } else {
	// TODO: spawn particles

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

    GameObject broken = Instantiate(broken_obj, this.transform.position, initRotation.rotation);
    //broken.SetActive(true);
    Destroy(this.gameObject);


    Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

    foreach (Rigidbody crb in rbs) {
      Vector3 blast_dir = punch_dir;

      //crb.constraints = RigidbodyConstraints.None;
      crb.isKinematic = false;
      crb.AddForce(blast_dir.normalized * power);
      //crb.gameObject.layer = LayerMask.NameToLayer("Punchable");
    }

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
