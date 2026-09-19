using UnityEngine;

class GhostUtils {
	public static float TopDownDistance(Vector3 v1, Vector3 v2) {
		return new Vector3(v1.x-v2.x, 0, v1.z-v2.z).magnitude;
	}
}
