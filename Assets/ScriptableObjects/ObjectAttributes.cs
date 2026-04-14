
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "ObjectAttributes", menuName = "Scriptable Objects/ObjectAttributes")]
public class ObjectAttributes : ScriptableObject
{
	[Header("Light Objects")]
	public float LIGHT_POISE_DAMAGE;
	public float LIGHT_GHOST_DAMAGE;
	public float LIGHT_OBJECT_DAMAGE;
	public float LIGHT_FORCE;
	[Header("Medium Objects")]
	public float MEDIUM_POISE_DAMAGE;
	public float MEDIUM_GHOST_DAMAGE;
	public float MEDIUM_OBJECT_DAMAGE;
	public float MEDIUM_FORCE;
	[Header("Heavy Objects")]
	public float HEAVY_POISE_DAMAGE;
	public float HEAVY_GHOST_DAMAGE;
	public float HEAVY_OBJECT_DAMAGE;
	public float HEAVY_FORCE;
	[Header("Very Heavy Objects")]
	public float VERY_HEAVY_POISE_DAMAGE;
	public float VERY_HEAVY_GHOST_DAMAGE;
	public float VERY_HEAVY_OBJECT_DAMAGE;
	public float VERY_HEAVY_FORCE;
}
