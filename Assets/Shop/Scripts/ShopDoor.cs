using UnityEngine;

public class ShopDoor : MonoBehaviour
{
    void Start() { }

    void Update() { }

		public void MouseOver() {
			HingeJoint hinge = GetComponent<HingeJoint>();
			JointSpring spring = hinge.spring;
			spring.targetPosition = -20.0f;
			// For some fucking reason you have to re-assign the object? Idk.
			hinge.spring = spring;
	 	}

		public void MouseOut() {
			HingeJoint hinge = GetComponent<HingeJoint>();
			JointSpring spring = hinge.spring;
			spring.targetPosition = 0.0f;
			hinge.spring = spring;
	 	}

}
