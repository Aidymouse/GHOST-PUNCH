using UnityEngine;

public class StaminaOrb : MonoBehaviour
{

		//public float max_stamina;
		public float mask_max_width;
		public float mask_min_width;
		public RectTransform mask;
		public GameObject full_img;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
		void Awake() {
		}

    void Start() { }

    // Update is called once per frame
    void Update() { }

		public void SetPortion(float p) {
			if (p >= 1) {
				mask.sizeDelta = new Vector2(mask_max_width, 100);
				full_img.SetActive(true);
			} else {
				full_img.SetActive(false);
				mask.sizeDelta = new Vector2(Lerp.lerp(mask_min_width, mask_max_width, p), 100);
			}
		}

}
