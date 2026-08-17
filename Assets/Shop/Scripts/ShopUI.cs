using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public interface ShopUIEventHandler : IEventSystemHandler {
  void ClickRight();
  void ClickLeft();
}

public class ShopUI : MonoBehaviour, ShopUIEventHandler
{

  public Shop shop;
	public ShopDoor shop_door;

	public TMP_Text item_board_title;
	public TMP_Text item_board_description;

  void Start() { 

	}
  void Update() { }

  public void ClickRight() {
		shop.LookRight();
  }

  public void ClickLeft() {
		shop.LookLeft();
  }

	/* Items */
	public void MouseOverItem(ShopItem item) {
		item_board_title.SetText(item.name);
		item_board_description.SetText(item.description);
	}

	public void MouseOutItem(ShopItem item) {
	}

	public void MouseDownItem(ShopItem item) {

		// TODO: spawn particles
		shop.BuyItem(item);

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


