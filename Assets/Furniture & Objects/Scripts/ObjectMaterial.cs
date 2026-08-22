using UnityEngine;

[CreateAssetMenu(fileName = "ObjectMaterial", menuName = "Scriptable Objects/ObjectMaterial")]
public class ObjectMaterial : ScriptableObject
{
	[Header("Sound")]
	[Tooltip("The sound that plays when the object is punched")]
	public AudioClip hit_sound;
	[Tooltip("The sound that plays when the object breaks")]
	public AudioClip break_sound;

	[Tooltip("High bound on pitch adjustment when sound plays")]
	public float pitch_high;
	[Tooltip("Low bound on pitch adjustment when sound plays")]
	public float pitch_low;

	[Header("Particles")]
	[Tooltip("Particles to spawn at hit location when the object is hit")]
	public ParticleSystem hit_particles;
	[Tooltip("Particles to spawn when the object breaks, if any")]
	public ParticleSystem break_particles;
	[Tooltip("[NOT FUNCTIONAL] Particles to spawn at the same time an object spawns")]
	public ParticleSystem spawn_particles;
    
}
