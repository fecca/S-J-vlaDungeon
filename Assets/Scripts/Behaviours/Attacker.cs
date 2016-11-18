using UnityEngine;

public class Attacker : MonoBehaviour, IAttacker
{
	[SerializeField]
	private AttackerData Data = null;

	private Transform _cachedTransform;
	private float _timer;

	private void Awake()
	{
		_cachedTransform = transform;
	}

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
			var targetPosition = target.position;
			targetPosition.y = _cachedTransform.position.y;
			Attack((targetPosition - _cachedTransform.position).normalized);
		}

		_timer += Time.deltaTime;
	}

	public void Attack(Vector3 direction)
	{
		var projectile = Instantiate(Data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, Data.ProjectileSpeed);
	}
}