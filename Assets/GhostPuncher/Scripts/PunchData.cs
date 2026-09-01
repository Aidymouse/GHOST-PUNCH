using UnityEngine;

public class Punch {
	public Punch(Vector3 dir, float f, float o_dmg, float g_dmg, float p_dmg, int hc) {
		direction = dir;
		force = f;
		object_damage = o_dmg;
		ghost_damage = g_dmg;
		poise_damage = p_dmg;
		hit_class = hc;
	}

	public static Punch FromData(Vector3 direction, PunchData data) {
		return new Punch(direction, data.force, data.object_damage, data.ghost_damage, data.poise_damage, data.hit_class);
	}


	public Vector3 direction;
	public float force;
	public float object_damage;
	public float poise_damage;
	public float ghost_damage;
	// 1st class punch is the strongest, 2nd class is a normal punch, 3 is big object, 4 is light object
	public int hit_class;
};

/*

*/
/** Scriptable object form of a punch */
[CreateAssetMenu(fileName = "PunchData", menuName = "Scriptable Objects/PunchData")]
public class PunchData : ScriptableObject
{
	[Tooltip("Physics force applied in direction of punch to objects")]
	public float force;
	[Tooltip("Amount of damage objects take from default punch")]
	public float object_damage;
	[Tooltip("Poise damage the ghost takes from a default punch")]
	public float poise_damage;
	[Tooltip("Damage the ghost takes from a default punch")]
	public float ghost_damage;
	[Tooltip("Class of hit for this punch (1=mega, 2=normal)")]
	public int hit_class;
}
