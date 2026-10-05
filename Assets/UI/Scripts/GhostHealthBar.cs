using UnityEngine;

public enum HealthBarOrientation {
	HORIZONTAL,
	VERTICAL
}

public class GhostHealthBar : MonoBehaviour {

	public RectTransform mask;
	public RectTransform mask_child;

	public float proportion = 1;
	public float prop_target = 1;
	public float speed = 6f;
	public HealthBarOrientation orientation;
	public Vector2 offset;

	[Tooltip("X or Y position of mask when bar is full")] public float mask_full;
	[Tooltip("X or Y position of mask when bar is empty")] public float mask_empty;
		//62.97
	[Tooltip("")] public float child_offset;
	//[Tooltip("X or Y position of bar graphic when bar is full")] public float bar_full;
	//[Tooltip("X or Y position of bar graphic when bar is empty")] public float bar_empty;

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
	public void UpdateProp(float proportion) {

		float prop = proportion % 1;
		if (prop == 0 && proportion != 0) {
			prop = 1;
		}

		float pos = Lerp.lerp(mask_empty, mask_full, prop);

		if (orientation == HealthBarOrientation.HORIZONTAL) {
			mask.anchoredPosition = new Vector2(pos, mask.anchoredPosition.y);
			mask_child.anchoredPosition = new Vector2(-pos + child_offset, mask_child.anchoredPosition.y);
		} else if (orientation == HealthBarOrientation.VERTICAL) {
			mask.anchoredPosition = new Vector2(mask.anchoredPosition.x, pos);
			mask_child.anchoredPosition = new Vector2(mask.anchoredPosition.x, -pos + child_offset);
		}
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
