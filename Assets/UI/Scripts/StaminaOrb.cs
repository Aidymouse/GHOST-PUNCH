using UnityEngine;

public class StaminaOrb : MonoBehaviour
{

		public float max_stamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
		void Awake() {
		}

    void Start() { }

    // Update is called once per frame
    void Update() { }

		public void SetPortion(float p) {
			this.GetComponent<RectTransform>().sizeDelta = new Vector2(100*p, 100);
		}

}
