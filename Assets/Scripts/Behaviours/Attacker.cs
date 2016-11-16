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

	public void Attack(Vector3 targetPosition)
	{
		var projectile = Instantiate(Resources.Load<GameObject>("Projectile"));
		targetPosition.y = transform.position.y;
		projectile.GetComponent<Projectile>().Setup(transform.position, targetPosition, Data.ProjectileSpeed);
	}
}