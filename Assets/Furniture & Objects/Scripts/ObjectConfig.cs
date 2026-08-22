
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "ObjectConfig", menuName = "Scriptable Objects/ObjectConfig")]
public class ObjectConfig : ScriptableObject
{
	[Tooltip("Height at which objects becomes walkthroughable")]
	public float WALKTHROUGH_HEIGHT;
}
