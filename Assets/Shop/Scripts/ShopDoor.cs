using UnityEngine;

public class ShopDoor : MonoBehaviour
{


		bool inCutscene = false;
	
		public GameObject black_bg;

		public HingeJoint hinge_left;
		public HingeJoint hinge_right;

    void Start() { 
		}

    void Update() { }
	
		// For some fucking reason you have to re-assign the object? Idk.
		void SetSpringAngle(HingeJoint joint, float new_angle) {
			JointSpring spring = joint.spring;
			spring.targetPosition = new_angle;
			joint.spring = spring;
		}

		public void MouseOver() {
			if (inCutscene) { return; }

			SetSpringAngle(hinge_right, -15.0f);
			SetSpringAngle(hinge_left, -15.0f);

	 	}

		public void MouseOut() {
			if (inCutscene) { return; }
			SetSpringAngle(hinge_right, 0.0f);
			SetSpringAngle(hinge_left, 0.0f);
	 	}

		public void StartRun() {
			inCutscene = true;

			SetSpringAngle(hinge_right, -90.0f);
			SetSpringAngle(hinge_left, -90.0f);

			// TODO: make this fade
			black_bg.SetActive(false);
		}

		public void EndRun() {
			inCutscene = false;

			SetSpringAngle(hinge_right, 0.0f);
			SetSpringAngle(hinge_left, 0.0f);

			black_bg.SetActive(true);
		}

}
