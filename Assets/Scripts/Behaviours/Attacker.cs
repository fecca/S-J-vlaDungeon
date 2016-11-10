using UnityEngine;

public class Attacker : MonoBehaviour, IAttacker
{
	[SerializeField]
	private AttackerData Data;

	private float _timer;

	public void Start()
	{
		_timer = Data.TimeBetweenAttacks;
	}
	public void Stop()
	{
	}
	public void UpdateBehaviour(Transform targetTransform)
	{
		if (_timer > Data.TimeBetweenAttacks)
		{
			_timer = 0;
			//CreateProjectile(targetTransform.position);
		}

		_timer += Time.deltaTime;
	}

	public void CreateProjectile(Vector3 targetPosition)
	{
		var projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
		projectile.transform.localScale = Vector3.one * 0.35f;
		projectile.transform.position = transform.position + Vector3.up;
		projectile.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		projectile.GetOrAddComponent<Rigidbody>().AddForce((targetPosition - transform.position).normalized * Data.ProjectileSpeed);
	}
}