using UnityEngine;

[CreateAssetMenu(fileName = "ShopDefaults", menuName = "Scriptable Objects/ShopDefaults")]
public class ShopDefaults : ScriptableObject
{
	[Tooltip("(rotations/s) Hovered item spin speed")] public float ITEM_SPIN_SPEED;
	[Tooltip("(m/s) Rate at which a purchased item shrinks")] public float ITEM_SHRINK_SPEED;
    
}
