using UnityEngine;

public class ShopCamera : MonoBehaviour
{
		public float turnSpeed;
		public Quaternion turnGoal;

		float turned;

    void Start()
    {
			turnGoal = this.transform.rotation;
        
    }

    void Update()
    {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, turnGoal, turnSpeed * Time.deltaTime);
    }

		public void LookTowardsShop() {
		}

		public void LookTowardsDoor() {
		}

		
}
