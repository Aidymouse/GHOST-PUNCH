using UnityEngine;
using Hairibar.Ragdoll.Animation;


public class RagdollAnimController : MonoBehaviour {
	
	public Animator anim;

	void Start() {
			ChangeAnimation("Idle");
	}


  public void ChangeAnimation(string new_anim, float fade_time=0f) {
    anim.CrossFade(new_anim, fade_time);
  }

	public void PlayAnimation(string animation) {
		Debug.Log("Playing " + animation);
		ChangeAnimation(animation);
	}

}
