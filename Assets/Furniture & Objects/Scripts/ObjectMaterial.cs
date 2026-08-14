using UnityEngine;

[CreateAssetMenu(fileName = "ObjectMaterial", menuName = "Scriptable Objects/ObjectMaterial")]
public class ObjectMaterial : ScriptableObject
{
	[Tooltip("The sound that plays when the object is punched")]
	public AudioClip hit_sound;
	public ParticleSystem hit_particles;
	public ParticleSystem break_particles;
	public ParticleSystem spawn_particles;
    
}
