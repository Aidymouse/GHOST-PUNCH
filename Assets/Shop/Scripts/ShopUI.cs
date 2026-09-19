using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public interface ShopUIEventHandler : IEventSystemHandler {
  void ClickRight();
  void ClickLeft();
}



/* Handles all player input events and handles how the shop responds */
public class ShopUI : MonoBehaviour, ShopUIEventHandler
{

  public Shop shop;
	public ShopDoor shop_door;

	public TMP_Text item_board_title;
	public TMP_Text item_board_description;
	public GameObject shrinking_item;


  void Start() { 

	}

  void Update() {
	}

  public void ClickRight() {
		shop.LookRight();
  }

  public void ClickLeft() {
		shop.LookLeft();
  }

	/* Items */
	public void MouseOverSlot(ShopSlot slot) {
		slot.MouseOver();

		if (slot.item) {
			UpdateBoard(slot.item);
		}

	}


	public void MouseDownSlot(ShopSlot slot) {
		slot.MouseDown();

		if (slot.item) {
			shop.BuyItem(slot.item);
		}
	}

	public void MouseOutSlot(ShopSlot slot) {
		slot.MouseOut();
		ClearBoard();
	}

	/* Board */
	public void UpdateBoard(ShopItem item) {
		item_board_title.SetText(item.name);
		item_board_description.SetText(item.description);
	}

	public void ClearBoard() {
			item_board_title.SetText("");
			item_board_description.SetText("");
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


