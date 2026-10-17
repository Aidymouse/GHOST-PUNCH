using UnityEngine;

public class AnimUtil {
	public static float GetClipLength(string anim_name, Animator animator) {

		RuntimeAnimatorController controller = animator.runtimeAnimatorController;


		foreach (AnimationClip clip in controller.animationClips) {
			if (clip.name == anim_name) { return clip.length; }
		}

		Debug.LogError("Tried to get clip length for " + anim_name + " but no clip by this name found in provided animatio");
		return 0;
		
	}
}
