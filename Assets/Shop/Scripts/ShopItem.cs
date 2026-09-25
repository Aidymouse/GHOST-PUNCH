using UnityEngine;

/* ShopItem is the monobehaviour that actually exists on the physical shop item in the game world.
 * Items also exist in state, which get applied to the ghost n stuff. That's just Item.cs, which is subclassed into individual items
 * */
public class ShopItem : MonoBehaviour
{
	public ItemType item_id;
	public int item_level;
	public int cost;
	public ShopDefaults shop_defaults;

	public string label;
	public string description;

	bool spinning;
	float spin_speed = 60f;

	bool shrinking = false;
	float shrink_speed = 1;

	void Awake() {
		spin_speed = shop_defaults.ITEM_SPIN_SPEED;
		shrink_speed = shop_defaults.ITEM_SHRINK_SPEED;
	}

	void Update() {
		if (spinning) {
			GetComponent<Transform>().Rotate(new Vector3(0, 0, spin_speed) * Time.deltaTime);
		}

		if (shrinking) {
			GetComponent<Transform>().localScale -= new Vector3(shrink_speed, shrink_speed, shrink_speed) * Time.deltaTime;

			if (GetComponent<Transform>().localScale.x < 0.1) {
				Destroy(this.gameObject);
			}
		
			
		}
	}

	public void StartSpinning() {
		spinning = true;
	}

	public void StopSpinning() {
		spinning = false;
	}

	public void ShrinkAndDisappear() {
		shrinking = true;
	}

}
