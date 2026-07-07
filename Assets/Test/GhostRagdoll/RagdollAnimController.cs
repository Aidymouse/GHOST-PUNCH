using UnityEngine;
using Hairibar.Ragdoll.Animation;


/* Script used to control ghost behaviour in ragdoll test scene */
public class RagdollAnimController : MonoBehaviour {
	
	public Animator anim;
	public RagdollAnimator rag;
	public Ghost ghost;

	float master_alpha;

	void Start() {
			master_alpha = rag.MasterAlpha;
			ChangeAnimation("Idle");
	}


  public void ChangeAnimation(string new_anim, float fade_time=0f) {
    anim.CrossFade(new_anim, fade_time);
  }

	public void PlayAnimation(string animation) {
		//Unragdoll();
		Debug.Log("Playing " + animation);
		ChangeAnimation(animation);
	}

	public void Ragdoll() {
		rag.MasterAlpha = 0;
	}

	public void Unragdoll() {
		rag.MasterAlpha = master_alpha;
	}

	public void PickNewSpot() {
		ghost.nav_destination = null;
		ghost.EnterAction(GhostActions.MOVING_ROOM);
	}

	public void GetUp(string dir) {
		ghost.EnterAction(GhostActions.GET_UP);
	}

}
