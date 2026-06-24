using UnityEngine;

public class SnapTo : MonoBehaviour
{
    public Vector3 snap_point;

		public void SnapToPoint(SnapTo snap) {
			SnapToPoint(snap.snap_point);
		}
		public void SnapToPoint(Vector3 point) {
			Vector3 offset = snap_point - point;
			//GetComponent<Transform>().Position += offset;
		}

}
