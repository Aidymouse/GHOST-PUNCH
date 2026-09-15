using UnityEngine;

/** @deprecated - i think? */
public class VanUI : MonoBehaviour {
	public ShopVan van;

	public void NextPage() {
		van.ChangeAnimation("VanDoorOpenAnim");
	}

	public void PrevPage() {
		van.ChangeAnimation("VanDoorCloseAnim");
	}

}
