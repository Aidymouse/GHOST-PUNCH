using UnityEngine;

/* ShopItem is the monobehaviour that actually exists on the physical shop item in the game world */
public class ShopItem : MonoBehaviour
{
	public ItemType item_id;
	public int item_level;
	public int cost;

	public string name;
	public string description;

	bool spinning;
	float spin_speed = 60f;

	void Update() {
		if (spinning) {
			GetComponent<Transform>().Rotate(new Vector3(0, 0, spin_speed) * Time.deltaTime);
		}
	}

	public void StartSpinning() {
		spinning = true;
	}

	public void StopSpinning() {
		spinning = false;
	}
}
