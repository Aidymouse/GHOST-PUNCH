using UnityEngine;
using UnityEngine.EventSystems;

public interface ShopUIEventHandler : IEventSystemHandler {
  void ClickRight();
  void ClickLeft();
}

public class ShopUI : MonoBehaviour, ShopUIEventHandler
{

  public Shop shop;
	public ShopDoor shop_door;

  void Start() { }
  void Update() { }

  public void ClickRight() {
		shop.LookRight();
  }

  public void ClickLeft() {
		shop.LookLeft();
  }

	/* Door */
	public void ClickDoor() {
		Debug.Log("Clicked Door");
		// TODO:
	}

	public void MouseOverDoor() {
		shop_door.MouseOver();
	}

	public void MouseOutDoor() {
		shop_door.MouseOut();
	}


	public void StartRun() {
		this.gameObject.SetActive(false);
	}

	public void EndRun() {
		this.gameObject.SetActive(true);
	}
}


