using UnityEngine;

public class Enemy : Character, IAttacking, IMoving
{
	private Transform _cachedTransform;
	private Brain _brain;
	private Transform _target;

	private void Awake()
	{
		_cachedTransform = transform;
		_target = FindObjectOfType<Player>().transform;
		_brain = new Brain(this, _target);
		_brain.EnterThought(ThoughtType.Idle);
	}
	private void Update()
	{
		_brain.Think();
	}

	public override void TakeDamage()
	{
		Agent.ClearNodes();
		Destroy(gameObject);
		MessageHub.Instance.Publish(new EnemyDiedEvent(null));
	}
	public void Attack(AttackerData data)
	{
		var targetPosition = _target.position;
		targetPosition.y = _cachedTransform.position.y;
		var direction = (targetPosition - _cachedTransform.position).normalized;
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, data.ProjectileSpeed);
	}
	public void Move(MoverData data)
	{
		Agent.StartPathTo(_target.position, data.MovementSpeed, () =>
		{
		});
	}
}