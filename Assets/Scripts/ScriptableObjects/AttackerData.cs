using UnityEngine;

[CreateAssetMenu]
public class AttackerData : ScriptableObject
{
	public float TimeBetweenAttacks = 1.0f;
	public float ProjectileSpeed = 15.0f;
	public GameObject ProjectilePrefab;
}