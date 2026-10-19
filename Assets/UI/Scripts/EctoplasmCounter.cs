using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EctoplasmCounter : MonoBehaviour
{

	float current;
	float target;
	float speed;
	[Tooltip("(ectopalsm/second) speed the counter counts up")]
	public float acceleration;
	public TMP_Text ectoplasm_text;



	public void Update() {

		if (target - current < speed * Time.deltaTime) {
			current = target;
			speed = 0;
		} else {
			speed += acceleration;
			current += speed * Time.deltaTime;
		}

		ectoplasm_text.SetText(""+Mathf.Floor(current));
	}

	public void SetTarget(float new_target) {
		target = new_target;
	}
}
		
