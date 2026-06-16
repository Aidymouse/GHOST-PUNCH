using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
		Slider slider;

		public bool useGradient;
		[Tooltip("Optional - Gradient to use as fill if present. Make sure there's a Fill image child")]
		public Gradient gradient;

		Image fill;

		public void Awake() {
			slider = this.GetComponent<Slider>();
			if (useGradient) {
				fill = this.GetComponentInChildren<Image>();
			}
		}

		public void SetMaxValue(float max)
		{
				slider.maxValue = max;
				slider.value = max;
				if (useGradient) {
					fill.color = gradient.Evaluate(slider.normalizedValue);
				}
		}

		public void SetValue(float val)
		{
				slider.value = Mathf.Max(val, 0);
				if (useGradient) {
					fill.color = gradient.Evaluate(slider.normalizedValue);
				}
		}
}
