using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using TMPro;

public class RandomBattleMusic : MonoBehaviour
{
    [Header("Battle Themes")]
    [SerializeField]
    private EventReference[] battleThemes;

    [Header("Song Title UI")]
    [SerializeField]
    private TMP_Text songNameText;

    [SerializeField]
    private CanvasGroup songNameCanvasGroup;

    [SerializeField]
    private float fadeInTime = 0.5f;

    [SerializeField]
    private float displayTime = 3f;

    [SerializeField]
    private float fadeOutTime = 1f;

    [Header("Settings")]
    [SerializeField]
    private bool playOnStart = true;

    [SerializeField]
    private bool preventSameSongTwice = true;

    private EventInstance currentSong;
    private int lastSongIndex = -1;

    private void Start()
    {
        // Make sure the title starts invisible
        if (songNameCanvasGroup != null)
        {
            songNameCanvasGroup.alpha = 0f;
        }

        if (playOnStart)
        {
            PlayRandomBattleTheme();
        }
    }

    public void PlayRandomBattleTheme()
    {
        if (battleThemes == null || battleThemes.Length == 0)
        {
            Debug.LogWarning(
                "RandomBattleMusic: No Battle Themes assigned."
            );

            return;
        }

        int randomIndex;

        // Pick a random song
        if (preventSameSongTwice && battleThemes.Length > 1)
        {
            do
            {
                randomIndex = Random.Range(
                    0,
                    battleThemes.Length
                );
            }
            while (randomIndex == lastSongIndex);
        }
        else
        {
            randomIndex = Random.Range(
                0,
                battleThemes.Length
            );
        }

        lastSongIndex = randomIndex;

        // Stop previous song
        StopCurrentSong();

        // Get selected FMOD event
        EventReference selectedSong = battleThemes[randomIndex];

        // Create and play FMOD event
        currentSong = RuntimeManager.CreateInstance(
            selectedSong
        );

        currentSong.start();

        // Show song title
        ShowSongName(selectedSong);
    }

    private void ShowSongName(EventReference song)
    {
        if (songNameText == null)
        {
            Debug.LogWarning(
                "RandomBattleMusic: Song Name Text is not assigned."
            );

            return;
        }

        if (songNameCanvasGroup == null)
        {
            Debug.LogWarning(
                "RandomBattleMusic: Canvas Group is not assigned."
            );

            return;
        }

        // Get the FMOD event path
        string songName = song.Path;

        // Get only the final part of the path
        if (songName.Contains("/"))
        {
            songName = songName.Substring(
                songName.LastIndexOf("/") + 1
            );
        }

        songNameText.text = songName;

        // Restart the fade animation
        StopAllCoroutines();
        StartCoroutine(FadeSongName());
    }

    private IEnumerator FadeSongName()
    {
        // Start invisible
        songNameCanvasGroup.alpha = 0f;

        // Fade IN
        float timer = 0f;

        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;

            float progress = timer / fadeInTime;

            songNameCanvasGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                progress
            );

            yield return null;
        }

        songNameCanvasGroup.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(displayTime);

        // Fade OUT
        timer = 0f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;

            float progress = timer / fadeOutTime;

            songNameCanvasGroup.alpha = Mathf.Lerp(
                1f,
                0f,
                progress
            );

            yield return null;
        }

        songNameCanvasGroup.alpha = 0f;
    }

    public void StopCurrentSong()
    {
        if (!currentSong.isValid())
        {
            return;
        }

        currentSong.stop(
            FMOD.Studio.STOP_MODE.ALLOWFADEOUT
        );

        currentSong.release();
        currentSong.clearHandle();
    }

    private void OnDestroy()
    {
        StopCurrentSong();
    }
}