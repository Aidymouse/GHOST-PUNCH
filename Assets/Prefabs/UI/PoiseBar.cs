using UnityEngine;
using UnityEngine.UI;

public class PoiseBar : MonoBehaviour
{

		public Slider slider;
		public Gradient gradient;
		public Image fill;

		public void SetMaxPoise(float poise)
		{
				slider.maxValue = poise;
				slider.value = poise;
				fill.color = gradient.Evaluate(1f);
		}

		public void SetPoise(float poise)
		{
				slider.value = poise;
				fill.color = gradient.Evaluate(slider.normalizedValue);
		}

}
