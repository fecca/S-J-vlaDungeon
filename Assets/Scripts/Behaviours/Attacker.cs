using UnityEngine;

public class Attacker : MonoBehaviour, IAttacker
{
	[SerializeField]
	private AttackerData Data;

	public void Attack(Vector3 position)
	{
		var projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
		projectile.transform.localScale = Vector3.one * 0.35f;
		projectile.transform.position = transform.position + Vector3.up;
		projectile.GetOrAddComponent<Rigidbody>().AddForce((position - transform.position).normalized * Data.ProjectileSpeed);
	}
}