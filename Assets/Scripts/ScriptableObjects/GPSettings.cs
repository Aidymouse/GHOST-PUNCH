using UnityEngine;

[CreateAssetMenu(fileName = "GPSettings", menuName = "Scriptable Objects/GPSettings")]
public class GPSettings : ScriptableObject
{

	[Tooltip("If true, stored settings will be overwritten with whatever you have set")]
	public bool force_reload_settings;

	public float mouse_sensitivity;
    
}
