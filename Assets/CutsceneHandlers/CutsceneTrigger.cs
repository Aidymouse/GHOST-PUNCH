using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector director;
    public GhostPuncher player;

    bool hasPlayed;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (!other.CompareTag("GhostPuncher"))
        {
            Debug.Log("Not player, ignored");
            return;
        }

        hasPlayed = true;

        player.inCutscene = true;

        director.Play();
    }
}