using UnityEngine;

public class FootballCollider : MonoBehaviour {

	public void OnCollisionEnter(Collision col) {
		Debug.Log("Hit something!");
	}

}
