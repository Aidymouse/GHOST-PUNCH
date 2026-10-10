using UnityEngine.AI;
using UnityEngine;

public static class NavMeshUtil {

	/** Get random point within sphere of origin and get the nearest point to it on the nav mesh
 * WARN: doesn't check the agent can actually get to the point */
	public static Vector3? GetRandomPoint(Vector3 origin, float maxRange, int max_tries=10) {

		for (int tries=0; tries < max_tries; tries++) {

			NavMeshHit hit; 

			bool found_pos = NavMesh.SamplePosition(
					origin + Random.insideUnitSphere * maxRange,
					out hit,
					maxRange,
					NavMesh.AllAreas // TODO: ghost area?
					);

			if (found_pos) {
				return hit.position;
			}

		}

		return null;
	}
}
