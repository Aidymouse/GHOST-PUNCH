using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PuncherDefaults", menuName = "Scriptable Objects/PuncherDefaults")]
public class PuncherDefaults : ScriptableObject
{
	[Header("Movement")]
	[Tooltip("Movement speed")]
	public float MOVE_SPEED;

	[Tooltip("Starts after PUNCH_COOLDOWN. If punching during this duration, the punch chain continues rather than starting again")]
	public float PUNCH_AGAIN;
	[Tooltip("If punch is clicked while cooldown has this time or less left, the punch input is buffered")]
	public float PUNCH_BUFFER_TIME;
	[Tooltip("Range of the punch in default unity units")]
	public float PUNCH_RANGE;


	[Header("Stamina")]
	[Tooltip("Maximum stamina for ghost puncher with no items")]
	public float BASE_STAMINA;
	[Tooltip("How much stamina you gain if you hit something with a punch")]
	public float STAMINA_GAINED_ON_HIT;
	[Tooltip("How long to wait in between stamina usage and starting to recharge")]
	public float STAMINA_RECHARGE_DELAY;
	[Tooltip("Stamina per second regained")]
	public float STAMINA_RECHARGE_RATE;

	[Header("Punch")]
	[Tooltip("Time after clicking punch that you can't punch again")]
	public float PUNCH_COOLDOWN;
	[Tooltip("Physics force applied in direction of punch to objects")]
	public float PUNCH_FORCE;
	[Tooltip("Amount of damage objects take from default punch")]
	public float PUNCH_OBJECT_DAMAGE;
	[Tooltip("Damage the ghost takes from a default punch")]
	public float PUNCH_GHOST_DAMAGE;
	[Tooltip("Poise damage the ghost takes from a default punch")]
	public float PUNCH_POISE_DAMAGE;
	[Tooltip("Stamina used on a punch")]
	public float PUNCH_STAMINA;
	[Tooltip("How much this contributes to the fear meter")]
	public float PUNCH_FEAR;


	[Header("Mega punch")]
	[Tooltip("Time after a megapunch that you can't punch (or use any abilities?)")]
	public float MEGAPUNCH_COOLDOWN;
	public float MEGAPUNCH_FORCE;
	public float MEGAPUNCH_OBJECT_DAMAGE;
	[Tooltip("Damage the ghost takes from a mega punch")]
	public float MEGAPUNCH_GHOST_DAMAGE;
	public float MEGAPUNCH_POISE_DAMAGE;
	[Tooltip("Stamina used on a mega punch")]
	public float MEGAPUNCH_STAMINA;
	public float MEGAPUNCH_FEAR;

	[Header("Fear Meter")]
	[Tooltip("The multipliers applied to damage. multipliers[n] required fear_required[n] fear to be attained")]
	public List<float> FEAR_MULTIPLIERS;
	[Tooltip("Fear required to attain the multiplier at N. Fear starts from 0 at each stage")]
	public List<float> FEAR_REQUIRED;
	[Tooltip("(seconds) Time that a combo remains, resets when hitting ghost or furniture")]
	public List<float> FEAR_RESET_TIMERS;

	[Header("Football Charge")]
	[Tooltip("The init speed of the charge")]
	public float CHARGE_START_SPEED;
	[Tooltip("The desired speed of the charge")]
	public float CHARGE_MAX_SPEED;
	[Tooltip("Charge accelerates at this rate at the start of the charge")]
	public float CHARGE_ACCELERATION;
	[Tooltip("Multiplier on moving left/right while charge active")]
	public float CHARGE_MOVE_LEFT_RIGHT_DAMPING;
	[Tooltip("Multiplier on looking left/right while charge active")]
	public float CHARGE_LOOK_LEFT_RIGHT_DAMPING;
	[Tooltip("Multiplier on looking up/down while charge active")]
	public float CHARGE_LOOK_UP_DOWN_DAMPING;
	[Tooltip("Stamina drain per second while charging")]
	public float CHARGE_STAMINA_DRAIN;
	[Tooltip("Maximum object height we can hit before stopping")]
	public float CHARGE_MAX_HEIGHT;
	[Tooltip("Time spent charging up before we start running")]
	public float CHARGE_CHARGE_TIME;
	[Tooltip("Time it takes to slide to a stop")]
	public float CHARGE_STOP_TIME;
	[Tooltip("Time after clicking punch that it actually goes through")]
	public float CHARGE_PUNCH_DELAY;
	[Tooltip("Time after charge punching it takes to stop (very quick I imagine)")]
	public float CHARGE_PUNCH_STOP;

}
