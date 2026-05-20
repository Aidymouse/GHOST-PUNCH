using UnityEngine;

public class CutsceneReceiver : MonoBehaviour
{
    public GhostPuncher player;

    public void StartCutscene()
    {
        player.inCutscene = true;
    }

    public void EndCutscene()
    {
        player.inCutscene = false;
    }
}