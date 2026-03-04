using UnityEngine;


public class BreakableObject : MonoBehaviour
{

  public GameObject broken_obj;
  public float hp;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void GetPunched(float damage, Vector3 punch_dir) {

    if (hp > 0) {
      hp -= damage;
      if (hp <= 0) {
	Break(punch_dir);
      } else {
	// TODO: spawn particles
      }
    }


  }

  void Break(Vector3 punch_dir) {
    GameObject broken = Instantiate(broken_obj, this.transform.position, this.transform.rotation);
    //broken.SetActive(true);
    Destroy(this.gameObject);


    Rigidbody[] rbs = broken.GetComponentsInChildren<Rigidbody>();

    foreach (Rigidbody crb in rbs) {
      Vector3 blast_dir = punch_dir;

      //crb.constraints = RigidbodyConstraints.None;
      crb.isKinematic = false;
      //crb.AddForce(blast_dir.normalized * 200);
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
