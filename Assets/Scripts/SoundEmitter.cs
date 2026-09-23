using UnityEngine;




public class SoundEmitter : MonoBehaviour {

	/* PlayAtPoint doesn't give you pitch controls! This essentially does the same thing but has Pitch Variation built in! */
	public static AudioSource PlayVariedSoundAtPoint(AudioClip clip, Vector3 position, float pitch_low, float pitch_high) {
		GameObject temp_source_obj = new GameObject();
		temp_source_obj.transform.position = position;
		AudioSource temp_source = temp_source_obj.AddComponent<AudioSource>();
		temp_source.clip = clip;
		temp_source.pitch = Random.Range(pitch_low, pitch_high);
		temp_source.Play();
		Destroy(temp_source_obj, clip.length);
		return temp_source;
	}

	AudioSource audio_source;

	/* @deprecated - we dont rly need this anymore I think */
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
