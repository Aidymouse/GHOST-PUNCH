using UnityEngine;

public class PuncherAbility {

	protected GhostPuncher puncher;

	/* Control vars for ghost punchers behaviour */
	public bool lock_punch = false;
	public bool lock_movement = false;

	public PuncherAbility(GhostPuncher p) {
		puncher = p;
	}

	public virtual void EnterAbility() {}
	public virtual void Update() {}
	public virtual void Activate() {}
	public virtual void End() {}
	public virtual void ExitAbility() {}
	/** For abilities that need to mess with movement (e.g. football charge), this method is available 
 	* Default is just pass through, so we can always call it
 	* **/
	public virtual Vector3 GetDesiredControlVec(Vector3 desired_vec) { return desired_vec; }


}

