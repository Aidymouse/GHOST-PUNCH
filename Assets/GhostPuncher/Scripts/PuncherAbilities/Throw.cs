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

		Collider[] punched = Physics.OverlapBox(puncher.punch_hitbox.transform.position, puncher.punch_hitbox.transform.localScale/2, puncher.punch_hitbox.transform.rotation, puncher.punchables_mask);		
	
		foreach (Collider col in punched) {
			BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
			if (!bo) { continue; }
			if (bo.attrs.GRABBABLE != Grabbable.NORMAL) { continue; }

			bo.GetGrabbed(puncher.throw_parent);
			puncher.held_object = bo.gameObject;

			break;
		}

		Debug.Log("Grab hit " + punched.Length + " things");
		
		// Set postion + rotation
		// Parent to the right hand bone in the camera rig... kind of awkward
		// Set the layer to viewmodel
		// Set the RB to kinematic

		puncher.ExitAbility();
	}
	
	void ThrowHeld() {
		
		// To activate infinite beers mode: Uncomment these lines, comment out the line 4 lines down and held_object = null at the bottom.
		//GameObject thrown = GameObject.Instantiate(puncher.held_object);
		//thrown.transform.localScale = new Vector3(100, 100, 100);

		GameObject thrown = puncher.held_object;

		/** Spawn thrown thing **/
		// Should this be done on breakable object script ?
		thrown.transform.position = puncher.throw_point.position;
		thrown.transform.SetParent(null);

		Vector3 throw_dir = puncher.throw_point.transform.TransformDirection(Vector3.forward);

		BreakableObject bo = thrown.GetComponent<BreakableObject>();
		if (bo) {
			bo.GetThrown(throw_dir.normalized * puncher.defaults.THROW_FORCE, new Vector3(1, 0, 0)*puncher.defaults.THROW_TORQUE);
		}


		puncher.held_object = null;

		puncher.ExitAbility();
	}


}
