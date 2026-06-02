using UnityEngine;

public class Shop : MonoBehaviour
{
  public float turnSpeed;
  public Quaternion turnGoal;

  public Camera shopCamera;

  public Transform camera_target;


  void Start()
  {
    camera_target.position = shopCamera.transform.position + Vector3.forward * 10;

  }

  void Update() { }

  public void LookTowardsShop() { }

  public void LookTowardsDoor() { }


}
