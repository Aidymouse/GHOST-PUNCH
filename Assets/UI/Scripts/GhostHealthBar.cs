using UnityEngine;

public class GhostHealthBar : MonoBehaviour {

	public RectTransform mask;
	public RectTransform mask_child;

	public float proportion = 1;
	public float prop_target = 1;
	public float speed = 6f;


	/*
 	* @param prop - The proportion of remaining health!
 	*/
	public void SetProportion(float prop) {
		prop = prop;
		prop_target = prop;
	}

	/*
 	* @param prop - The proportion of remaining health!
 	*/
	public void UpdateProp(float prop) {
		mask.anchoredPosition = new Vector2(-mask.rect.width * (1-prop), mask.anchoredPosition.y);
		mask_child.anchoredPosition = new Vector2(mask.rect.width * (1-prop), mask_child.anchoredPosition.y);
	}

	/*
 	* @param prop - The proportion of remaining health!
 	*/
	public void SetPropTarget(float prop) {
		prop_target = prop;
	}

	public void Update() {
		float prop_change = speed * Time.deltaTime;
		float change_required = Mathf.Abs(proportion - prop_target);

		if (prop_change > change_required) {
			proportion = prop_target;
		} else {
			if (prop_target > proportion) {
				proportion += prop_change;
			} else if (prop_target < proportion) {
				proportion -= prop_change;
			}
		}

		UpdateProp(proportion);
	}
}
