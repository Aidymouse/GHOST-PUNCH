using UnityEngine;

[CreateAssetMenu(fileName = "ShopSounds", menuName = "Scriptable Objects/ShopSounds")]
public class ShopSounds : ScriptableObject
{
	[Tooltip("Plays when purchasing an item")]
	public AudioClip PURCHASE;
	[Tooltip("Plays when hovering an item")]
	public AudioClip ITEM_HOVER;

	[Header("Tip jar - TODO")]
	[Tooltip("Plays when clicking the tip jar")]
	public AudioClip TIP;
	[Tooltip("Plays when the goo splases into the tip jar")]
	public AudioClip TIP_GOO_SPLASH;
    
}
