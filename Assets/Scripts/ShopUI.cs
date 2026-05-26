using UnityEngine;
using UnityEngine.EventSystems;

public interface ShopUIEventHandler : IEventSystemHandler {
  void ClickRight();
  void ClickLeft();
}

public class ShopUI : MonoBehaviour, ShopUIEventHandler
{

  public Shop shop;

  void Start()
  {
  }

  void Update()
  {

  }

  public void ClickRight() {
    Vector3 new_pos_dir = shop.camera_target.transform.TransformDirection(Vector3.right);
    shop.camera_target.position = shop.shopCamera.transform.position + new_pos_dir * 10;
  }

  public void ClickLeft() {
    Vector3 new_pos_dir = shop.camera_target.transform.TransformDirection(Vector3.left);
    shop.camera_target.position = shop.shopCamera.transform.position + new_pos_dir * 10;
  }
}
