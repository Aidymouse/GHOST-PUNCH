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

	bool shrinking = false;

	void Update() {
		if (spinning) {
			GetComponent<Transform>().Rotate(new Vector3(0, 0, spin_speed) * Time.deltaTime);
		}

		if (shrinking) {
			GetComponent<Transform>().localScale -= new Vector3(1, 1, 1) * Time.deltaTime;

			if (GetComponent<Transform>().localScale.x < 0.1) {
				Destroy(this);
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
