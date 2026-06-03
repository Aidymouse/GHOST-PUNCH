using UnityEngine;

public enum ShopItemIDs {
	SALT_SHAKER,
	PAINKILLERS,
	GHOST_DETECTOR,
};

public class ShopItem : MonoBehaviour
{

	public ShopItemIDs item_id;
	public int cost;

}
