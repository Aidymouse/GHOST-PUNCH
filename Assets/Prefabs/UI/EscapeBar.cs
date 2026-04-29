using UnityEngine;
using UnityEngine.UI;

public class EscapeBar : MonoBehaviour
{

		//this should probably be like a clock or something but I want a bar for now
		public Slider slider;
		public Gradient gradient;
		public Image fill;

		public void SetMaxEscape(float escape)
		{
				slider.maxValue = escape;
				fill.color = gradient.Evaluate(1f);
		}

		public void SetEscape(float escape)
		{
				slider.value = escape;
				fill.color = gradient.Evaluate(slider.normalizedValue);
		}

}
