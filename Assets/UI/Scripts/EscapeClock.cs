using UnityEngine;
using UnityEngine.UI;

public class EscapeClock : MonoBehaviour
{
		public Image clock_hand;

		public void SetTimeLeft(float per) {
			clock_hand.transform.eulerAngles = new Vector3(0, 0, 360 * per);
		}
}
		
