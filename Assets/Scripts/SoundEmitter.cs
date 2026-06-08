using UnityEngine;

public class SoundEmitter : MonoBehaviour {

	AudioSource audio_source;

	public static GameObject Create(AudioClip clip) {
		GameObject g = Instantiate(new GameObject());

		AudioSource a = g.AddComponent<AudioSource>();

		SoundEmitter gse = g.AddComponent<SoundEmitter>();
		gse.Start();
		gse.PlaySound(clip);

		return g;
	}

	public void Start() {
		audio_source = GetComponent<AudioSource>();
	}

	public void PlaySound(AudioClip clip) {
		audio_source.clip = clip;
		audio_source.Play();
	}

	public void Update() {
		if (!audio_source.isPlaying) { Destroy(this); }
	}
	

}
