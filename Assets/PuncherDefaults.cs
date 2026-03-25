using UnityEngine;

[CreateAssetMenu(fileName = "PuncherDefaults", menuName = "Scriptable Objects/PuncherDefaults")]
public class PuncherDefaults : ScriptableObject
{
	[Tooltip("Movement speed")]
	public float MOVE_SPEED;
	[Tooltip("Time after clicking punch that you can't punch again")]
	public float PUNCH_COOLDOWN;
	[Tooltip("Starts after PUNCH_COOLDOWN. If punching during this duration, the punch chain continues rather than starting again")]
	public float PUNCH_AGAIN;
	[Tooltip("If punch is clicked while cooldown has this time or less left, the punch input is buffered")]
	public float PUNCH_BUFFER_TIME;

	[Tooltip("Power of a punch. Power is damage dealt as well as force applied on hit to physics objects")]
	public float PUNCH_POWER;
	[Tooltip("Power of a charge punch. Power is damage dealt as well as force applied on hit to physics objects")]
	public float PUNCH_MEGA_POWER;
    
}
