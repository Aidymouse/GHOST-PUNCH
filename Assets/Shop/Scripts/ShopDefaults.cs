using UnityEngine;

[CreateAssetMenu(fileName = "ShopDefaults", menuName = "Scriptable Objects/ShopDefaults")]
public class ShopDefaults : ScriptableObject
{
	[Tooltip("Time for item to shrink after purchase")] public float SHRINK_TIME;
    
}
