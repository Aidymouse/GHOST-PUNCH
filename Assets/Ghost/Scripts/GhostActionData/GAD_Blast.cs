
using UnityEngine;

[CreateAssetMenu(fileName = "GAD_Blast", menuName = "Scriptable Objects/GhostActionData/Blast")]
public class GAD_Blast : ScriptableObject
{
	[Tooltip("Force that pushes you away")]
	public float POWER;
	[Tooltip("How quickly the wave loses power when pushing")]
	public float DECAY;
	[Tooltip("Threshold at which push power is so weak it no longer pushes ghost puncher back")]
	public float POWER_THRESHOLD;

	[Tooltip("Orb the ghost spawns when the blast anim completes")]
	public GameObject ORB;

	[Tooltip("Should sync the ghosts blast anim so the orb is fired when the blast goes off")]
	public float CHARGE_TIME;
	[Tooltip("Time the ghost spends waiting for the blast")]
	public float POST_BLAST_WAIT;

}
