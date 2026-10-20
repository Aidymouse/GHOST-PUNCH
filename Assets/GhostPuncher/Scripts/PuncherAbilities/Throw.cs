using UnityEngine;

public class Throw : PuncherAbility {

	//Timer ti_grab;
	Timer ti_grab_delay;
	Timer ti_throw_delay;

	PAD_Throw throw_data;
	
	public Throw(GhostPuncher p) : base(p) {
		//float grab_len = AnimUtil.GetClipLength("Grab", p.arm_animator);
		//Debug.Log("Grab len " + grab_len);
		//ti_grab = new Timer(0, grab_len);

		throw_data = p.defaults.throw_data;

		ti_grab_delay = new Timer(0, throw_data.GRAB_DELAY);
		ti_throw_delay = new Timer(0, throw_data.THROW_DELAY);

	}

	public override void EnterAbility() {
		if (!puncher.held_object) {
			StartGrab();
		} else {
			StartThrow();
		}
	}

	public override void Update() {
		ti_grab_delay.Tick(Time.deltaTime);
		if (ti_grab_delay.FinishedThisFrame()) {
			Grab();
		}

		ti_throw_delay.Tick(Time.deltaTime);
		if (ti_throw_delay.FinishedThisFrame()) {
			ThrowHeld();
		}
	}

	void StartGrab() {
		ti_grab_delay.Reset();
		if (ti_grab_delay.Finished()) {
			Grab();
		}
	}

	void Grab() {
		// basically find all punch targets and pick the one we're the most looking at up
		puncher.ChangeAnimation("Grab");

		Collider[] punched = Physics.OverlapBox(puncher.punch_hitbox.transform.position, puncher.punch_hitbox.transform.localScale/2, puncher.punch_hitbox.transform.rotation, puncher.punchables_mask);		
	
		foreach (Collider col in punched) {
			BreakableObject bo = col.gameObject.GetComponent<BreakableObject>();
			if (!bo) { continue; }
			if (bo.attrs.GRABBABLE != Grabbable.NORMAL) { continue; }

			bo.GetGrabbed(puncher.throw_parent);
			puncher.held_object = bo.gameObject;

			break;
		}

		// Control animator
		puncher.arm_animator.SetFloat("HeldItem", 0);
		if (puncher.held_object != null) {
			puncher.arm_animator.SetFloat("HeldItem", 1);
		}

		// Either reset timer to wait for grab anim to finish, or just exit action
		//ti_grab.Reset();
		puncher.ExitAbility();
	}

	void StartThrow() {
		ti_throw_delay.Reset();
		puncher.ChangeAnimation("Throw");
		if (ti_throw_delay.Finished()) {
			ThrowHeld();
		}
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
			bo.GetThrown(throw_dir.normalized * throw_data.THROW_FORCE, new Vector3(1, 0, 0)*throw_data.THROW_TORQUE);
		}


		puncher.held_object = null;
		puncher.arm_animator.SetFloat("HeldItem", 0);

		puncher.ExitAbility();
	}


}
