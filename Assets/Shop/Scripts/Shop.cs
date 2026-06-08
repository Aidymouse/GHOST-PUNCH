using UnityEngine;

public class Shop : MonoBehaviour
{
  public float turnSpeed;
  public Quaternion turnGoal;

  public Transform camera_pos;
  public Transform camera_target;


  void Start()
  {
    camera_target.position = camera_pos.position + Vector3.forward * 10;

  }

  void Update() { }

  public void LookTowardsShop() { }

  public void LookTowardsDoor() { }

	/* Item Management */
	public void BuyItem(ShopItem item) {
		Debug.Log(item.item_id + " costs " + item.cost + " ectoplasm");
	}

}
