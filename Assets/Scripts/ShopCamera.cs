using UnityEngine;

public class ShopCamera : MonoBehaviour
{
		public float turnSpeed;
		public Quaternion turnGoal;

		float turned;

    void Start()
    {
        
    }

    void Update()
    {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, turnGoal, turnSpeed*Time.deltaTime);
    }

		void LookTowardsShop() {
			turnGoal.y = 90;
		}

		void LookTowardsDoor() {
			turnGoal.y = 0;
		}

		
}
