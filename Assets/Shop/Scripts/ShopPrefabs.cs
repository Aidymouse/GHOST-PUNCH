using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopPrefabs", menuName = "Scriptable Objects/ShopPrefabs")]
public class ShopPrefabs : ScriptableObject
{
	/* Indexed into via item type enum */
	public List<GameObject> item_prefabs;
    
}
