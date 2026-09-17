using UnityEngine;

public class Throw : PuncherAbility {

	public Throw(GhostPuncher p) : base(p) { }

	public override void EnterAbility() {
		if (!puncher.held_object) {
			Grab();
		} else {
			ThrowHeld();
		}
	}

	void Grab() {
		// basically find all punch targets and pick the one we're the most looking at up
		// Set postion + rotation
		// Parent to the right hand bone in the camera rig... kind of awkward
		// Set the layer to viewmodel
		// Set the RB to kinematic
	}
	
	void ThrowHeld() {
		
		// To activate infinite beers mode: Uncomment these lines, comment out the line 4 lines down and held_object = null at the bottom.
		//GameObject thrown = GameObject.Instantiate(puncher.held_object);
		//thrown.transform.localScale = new Vector3(100, 100, 100);

		GameObject thrown = puncher.held_object;

		/** Spawn thrown thing **/
		thrown.transform.position = puncher.throw_point.position;
		thrown.transform.SetParent(null);

		thrown.layer = LayerMask.NameToLayer("FlyingObject");

		Vector3 throw_dir = puncher.throw_point.transform.TransformDirection(Vector3.forward);

		Rigidbody rb = thrown.GetComponent<Rigidbody>();
		if (rb) {
			rb.isKinematic = false;
			rb.AddForce(throw_dir.normalized * puncher.defaults.THROW_FORCE);
			rb.AddTorque(new Vector3(1, 0, 0)*puncher.defaults.THROW_TORQUE);
		}

		BreakableObject bo = thrown.GetComponent<BreakableObject>();
		if (bo) {
			bo.enabled = true;
		}


		puncher.held_object = null;

		puncher.ExitAbility();
	}


}
