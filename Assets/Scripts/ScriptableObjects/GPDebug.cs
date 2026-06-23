using UnityEngine;

[CreateAssetMenu(fileName = "GPDebug", menuName = "Scriptable Objects/GPDebug")]
public class GPDebug : ScriptableObject
{

	[Header("Ghost")]
	[Tooltip("If true, the ghost won't end the run when it's escape meter is full")]
	public bool dont_end_run;
    
}
