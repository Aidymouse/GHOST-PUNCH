using UnityEngine;

[CreateAssetMenu(fileName = "GPDebug", menuName = "Scriptable Objects/GPDebug")]
public class GPDebug : ScriptableObject
{

	[Header("Ghost")]
	[Tooltip("If true, the ghost won't end the run when it's escape meter is full")]
	public bool dont_end_run;
	[Tooltip("")]
	public bool use_power_override;
	[Tooltip("Power override, for testing")]
	public GhostActions power_override;
    
}
