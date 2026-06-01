using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;

public class GhostJumpscareController : MonoBehaviour
{
    [Header("References")]
    public Ghost ghost;
    public Transform player;

    [Header("Alignment")]
    public Transform jumpscareTarget;

    [Header("Timeline")]
    public PlayableDirector timeline;

    [Header("Player Control")]
    public GhostPuncher playerController;

    [Header("Settings")]
    public float triggerDistance = 2f;

    bool active;

    void Update()
    {
        if (active) return;
        if (ghost == null || player == null) return;

        float dist = Vector3.Distance(ghost.transform.position, player.position);

        if (ghost.jumpscareReady && dist <= triggerDistance)
        {
            TriggerJumpscare();
        }

        // DEBUG TEST
        if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerJumpscare();
        }
    }

    public void TriggerJumpscare()
    {
        if (active) return;
        active = true;

        Debug.Log("JUMPSCARE TRIGGERED");

        // 1. Freeze player
        if (playerController != null) { playerController.inCutscene = true; }

        // 2. Freeze ghost AI completely
        NavMeshAgent agent = ghost.get_nav_agent();
        if (agent != null) {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 3. Stop animation control
        Animator anim = ghost.GetComponentInChildren<Animator>();
        if (anim != null) { anim.enabled = false; }

        // 4. Stop physics
        Rigidbody rb = ghost.rig_core;
        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 5. ALIGN using SAFE method (no offset math)
        if (jumpscareTarget != null && ghost.jumpscareAlignPoint != null)
        {
            Transform align = ghost.jumpscareAlignPoint;

            // move ROOT so ALIGN POINT matches target
            Vector3 offset = ghost.transform.position - align.position;

            ghost.transform.position = jumpscareTarget.position + offset;
        }

        // 6. Rotate toward player (optional cinematic look)
        Vector3 look = player.position - ghost.transform.position;
        look.y = 0;

        if (look.sqrMagnitude > 0.001f)
            ghost.transform.rotation = Quaternion.LookRotation(look);

        // 7. Play Timeline
        if (timeline != null)
        {
            timeline.time = 0;
            timeline.Evaluate();
            timeline.Play();
        }
    }

    // Call this from Timeline Signal at the end
    public void EndJumpscare()
    {
        Debug.Log("JUMPSCARE ENDED");

        active = false;

        // restore player control
        if (playerController != null)
            playerController.inCutscene = false;

        // restore ghost AI
        NavMeshAgent agent = ghost.get_nav_agent();
        if (agent != null)
            agent.enabled = true;

        Animator anim = ghost.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.enabled = true;

        ghost.cur_action = GhostActions.MOVING_ROOM;
    }
}
