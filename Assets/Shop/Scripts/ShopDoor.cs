using UnityEngine;

public class ShopDoor : MonoBehaviour
{


		bool inCutscene = false;
	
		HingeJoint hinge;
		public GameObject black_bg;

    void Start() { 
 			hinge = GetComponentInChildren<HingeJoint>();
		}

    void Update() { }

		public void MouseOver() {
			if (inCutscene) { return; }

			JointSpring spring = hinge.spring;
			spring.targetPosition = -20.0f;
			// For some fucking reason you have to re-assign the object? Idk.
			hinge.spring = spring;
	 	}

		public void MouseOut() {
			if (inCutscene) { return; }
			JointSpring spring = hinge.spring;
			spring.targetPosition = 0.0f;
			hinge.spring = spring;
	 	}

		public void StartRun() {
			inCutscene = true;

			JointSpring spring = hinge.spring;
			spring.targetPosition = -90.0f;
			hinge.spring = spring;

			black_bg.SetActive(false);
		}

}
