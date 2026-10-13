

using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Scurry", menuName = "Scriptable Objects/GhostActionData/Scurry")]
public class GAD_Scurry : ScriptableObject
{
		[Tooltip("Debug object I used to test getting random position")]
    public GameObject debug_spawn;
		[Tooltip("Name of the scurry animation")]
		public string animation_name;
		[Tooltip("Speed while scurrying")]
		public float speed;
		[Tooltip("Turn Speed while scurrying")]
		public float turn_speed;
}
