using UnityEngine;

// TODO:
public class GhostAction_Jumpscare : GhostAction {

	public GhostAction_Jumpscare(Ghost g) : base(g) {}

	public override void Enter() {
		// 1. Find spot behind ghost puncher
		// 2. Teleport behind him
		// 3. If the ghost puncher doesn't look long enough, jumpscare him
		// 3a. If the ghost puncher realises and looks too fast, flee to a point out of sight
		// 3b. If the ghost puncher waits just the right amount of time then turns, flinch so he can get some hits in
	}

	public override void Exit() {
	}

	public override void Update() {
		Debug.Log("TODO: implement jumpscare action");
	}
	
}

/*
  void CheckJumpscareTrigger()
  {
    if (!jumpscareReady) return;
    float dist = Vector3.Distance(transform.position, ghostPuncher.transform.position);

		if (dist <= jumpscareDistance)
		{
			TriggerJumpscare();
		}
	}

  public void TriggerJumpscare()
  {
    if (inJumpscare) return;
    inJumpscare = true;
    Debug.Log("JUMPSCARED");

    // Freeze AI
    nav_agent.isStopped = true;

    // align point to target, this doesn't fucking work.
    Vector3 offset = transform.position - jumpscareAlignPoint.position;
    transform.position = jumpscareTarget.position + offset;
    transform.rotation = jumpscareTarget.rotation;

    // Lock player movement
    ghostPuncher.inCutscene = true;

    // Play timeline
    jumpscareTimeline.time = 0;
    jumpscareTimeline.Play();
  }

  public void EndJumpscare()
  {
    inJumpscare = false;
    nav_agent.enabled = true;
    nav_agent.updatePosition = true;
    nav_agent.updateRotation = true;
    anim.enabled = true;
    ghostPuncher.GetComponent<GhostPuncher>().inCutscene = false;
  }
*/
