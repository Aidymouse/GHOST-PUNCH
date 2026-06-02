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
		Vector3 to_target = shop.camera_target.transform.position - shop.shopCamera.transform.position;
		Vector3 rotate = Vector3.Normalize((Quaternion.AngleAxis(90, Vector3.up) * to_target));
    shop.camera_target.position = shop.shopCamera.transform.position + rotate * 10;
  }

  public void ClickLeft() {
		Vector3 to_target = shop.camera_target.transform.position - shop.shopCamera.transform.position;
		Vector3 rotate = Vector3.Normalize((Quaternion.AngleAxis(-90, Vector3.up) * to_target));
    shop.camera_target.position = shop.shopCamera.transform.position + rotate * 10;
  }
}
