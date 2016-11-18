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
	public void UpdateBehaviour(Transform target)
	{
		if (_timer > Data.TimeBetweenAttacks)
		{
			_timer = 0;
			Attack(target.position);
		}

		_timer += Time.deltaTime;
	}

	public void Attack(Vector3 direction)
	{
		var projectile = Instantiate(Resources.Load<GameObject>("PlayerProjectile"));
		projectile.GetComponent<Projectile>().Setup(transform.position, direction, Data.ProjectileSpeed);
	}
}