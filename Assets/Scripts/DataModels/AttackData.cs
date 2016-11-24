using UnityEngine;

public class AttackData
{
	public float TimeBetweenAttacks { get; private set; }
	public float ProjectileSpeed { get; private set; }
	public GameObject ProjectilePrefab { get; private set; }

	public AttackData(float timeBetweenAttacks, float projectileSpeed, string projectileName)
	{
		TimeBetweenAttacks = timeBetweenAttacks;
		ProjectileSpeed = projectileSpeed;
		ProjectilePrefab = Resources.Load<GameObject>(projectileName);
	}
}