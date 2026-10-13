using UnityEngine.AI;
using UnityEngine;

public static class NavMeshUtil {

	/** Get random point within sphere of origin and get the nearest point to it on the nav mesh
 * WARN: doesn't check the agent can actually get to the point
 * NOTE: Doesn't necessarily get a point that is actually minRange away from target, because 
 * */
	public static Vector3? GetRandomPoint(Vector3 origin, float maxRange, float minRange=0.0f, bool force_min_range=false, int max_tries=10) {

		if (minRange >= maxRange) { return null; }


		for (int tries=0; tries < max_tries; tries++) {

			NavMeshHit hit; 

			Vector3 rand_point = Random.insideUnitSphere;
			Vector3 rand_dir = rand_point.normalized;
			// Takes advantage of fact that magnitude of rand_point is 0-1
			Vector3 point = rand_dir * minRange + rand_point * (maxRange - minRange);

			bool found_pos = NavMesh.SamplePosition(
					origin + point,
					out hit,
					maxRange,
					NavMesh.AllAreas // TODO: ghost area?
					);

			if (found_pos) {
				if (force_min_range && (origin - hit.position).magnitude < minRange) {
					continue;
				}

				return hit.position;
			}

		}

		return null;
	}
}
