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
	[Tooltip("Range of the punch in default unity units")]
	public float PUNCH_RANGE;
	[Tooltip("How much this contributes to the fear meter")]
	public float PUNCH_FEAR;


	[Tooltip("Maximum stamina to start with")]
	public float MAX_STAMINA;
	[Tooltip("How long to wait in between stamina usage and starting to recharge")]
	public float STAMINA_RECHARGE_DELAY;
	[Tooltip("Stamina per second regained")]
	public float STAMINA_RECHARGE_RATE;

	[Tooltip("Physics force applied in direction of punch to objects")]
	public float PUNCH_FORCE;
	[Tooltip("Amount of damage objects take from default punch")]
	public float PUNCH_OBJECT_DAMAGE;
	[Tooltip("Damage the ghost takes from a default punch")]
	public float PUNCH_GHOST_DAMAGE;
	[Tooltip("Poise damage the ghost takes from a default punch")]
	public float PUNCH_POISE_DAMAGE;
	public float PUNCH_STAMINA;

	public float MEGAPUNCH_FORCE;
	public float MEGAPUNCH_OBJECT_DAMAGE;
	[Tooltip("Damage the ghost takes from a mega punch")]
	public float MEGAPUNCH_GHOST_DAMAGE;
	public float MEGAPUNCH_POISE_DAMAGE;
	public float MEGAPUNCH_STAMINA;
	public float MEGAPUNCH_FEAR;
    
}
