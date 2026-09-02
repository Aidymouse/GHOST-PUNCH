using UnityEngine;
using Unity.Cinemachine;

/* Provides easy API for manipulating camera FOV */
public class FOVController : MonoBehaviour {
	public CinemachineCamera vcam;
	[HideInInspector] public float fov_orig;
	[HideInInspector] public float fov_target_offset;
	[HideInInspector] public float fov_speed;

	public void SetTargetFOVOffset(float target) { fov_target_offset = target; }


	/* Calibrates FOV speed such that it will get to provided offset in provided seconds */
	public void GetToOffsetIn(float offset, float seconds) {
		float cur_fov = vcam.Lens.FieldOfView;
		float fov_target = fov_orig + offset;
		float fovDistance = Mathf.Abs(fov_target - cur_fov);
		fov_speed = fovDistance / seconds;
		fov_target_offset = offset;

		//Debug.Log("To get from " + cur_fov + " to " + fov_target + " in " + seconds + " seconds, I will go " + fov_speed + " FOV per second");
	}

	/* Wrapper for above to get to 0 easy */
	public void GetToZeroIn(float seconds) { 
		GetToOffsetIn(0, seconds);
	}

	public void SetFOVSpeed(float speed) { fov_speed = speed; }

	/* Update the current FOV, which will immediately start trying to move back towards the target offset */
	public void SetFOVByOffset(float offset) {
		vcam.Lens.FieldOfView = fov_orig + offset;
	}

	/* Essentially a stable set on FOV */
	public void SetTargetAndFOVOffset(float target) {
		fov_target_offset = target;
		vcam.Lens.FieldOfView = fov_orig + target;
	}
	
	public void Awake() {
		fov_speed = 100;
		fov_target_offset = 0;
		fov_orig = vcam.Lens.FieldOfView;
	}

	public void Update() {
		
		float fov_target = fov_orig + fov_target_offset;

		float fovDelta = Mathf.Abs(fov_target - vcam.Lens.FieldOfView);

		if (fovDelta > 0.01) {
			if (fovDelta < fov_speed * Time.deltaTime) {
				vcam.Lens.FieldOfView = fov_target;
			} else {
				if (fov_target > vcam.Lens.FieldOfView) {
					vcam.Lens.FieldOfView += fov_speed * Time.deltaTime;
				} else {
					vcam.Lens.FieldOfView -= fov_speed * Time.deltaTime;
				}
			}	
		}

	}
}
