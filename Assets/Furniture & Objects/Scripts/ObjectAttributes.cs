using UnityEngine;
using UnityEngine.VFX;

public enum Grabbable {
	NORMAL,
	// TODO: not yet implemented
	SUPER,
	NO
}

[CreateAssetMenu(fileName = "ObjectAttributes", menuName = "Scriptable Objects/ObjectAttributes")]
public class ObjectAttributes : ScriptableObject
{
	[Tooltip("Damage dealt when flying into the ghost")]	
	public float GHOST_DAMAGE;
	[Tooltip("Poise damage dealt when flying into the ghost")]	
	public float POISE_DAMAGE;
	[Tooltip("Damage dealt to other objects when smashing into them")]
	public float OBJECT_DAMAGE;
	[Tooltip("Force imparted when smashing into an object, in addition to normal physics force")]
	public float FORCE;
	[Tooltip("Are objects of this attr grabbable? NORMAL = yes, SUPER = two-handed upgrade, NO = never")]
	public Grabbable GRABBABLE;
}
