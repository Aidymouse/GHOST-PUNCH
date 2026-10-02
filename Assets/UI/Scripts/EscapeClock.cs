using UnityEngine;
using UnityEngine.UI;

public class EscapeClock : MonoBehaviour
{
		public Image clock_hand;
		public Image clock_color;

		/** @param per - Percentage of time left, 0 when time starts and 1 when it ends */
		public void SetTimeLeft(float per) {
			
			clock_hand.transform.localRotation = Quaternion.Euler(0, 0, 360 - (360 * per));
			clock_color.fillAmount = per;
		}
}
		
