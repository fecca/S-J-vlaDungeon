using System;
using UnityEngine;

public class Enemy : Character, IAttacking, IMoving
{
	[SerializeField]
	private Canvas _canvas = null;
	private RectTransform _bar;

	private Transform _cachedTransform;
	private Brain _brain;
	private Transform _target;

	private void Awake()
	{
		_cachedTransform = transform;
		_target = FindObjectOfType<Player>().transform;
		_brain = new Brain(this, _target);
		_brain.EnterThought(ThoughtType.Idle);
		_bar = _canvas.transform.FindChild("CurrentHitpoints").GetComponent<RectTransform>();
	}
	private void Update()
	{
		_brain.Think();
		_canvas.transform.LookAt(Camera.main.transform.position);
	}

	public override void SetHealth(HealthData healthData)
	{
		Stats = new Stats(healthData);
	}
	public void SetToAttacker(AttackerData attackerData)
	{
		_brain.SetToAttacker(attackerData);
	}
	public void SetToMover(MoverData moverData)
	{
		_brain.SetToMover(moverData);
	}
	public override void TakeDamage()
	{
		Stats.CurrentHealth--;
		if (Stats.CurrentHealth > 0)
		{
			var fraction = Stats.CurrentHealth / Stats.TotalHitpoints;
			_bar.sizeDelta = new Vector2(fraction * 4, _bar.sizeDelta.y);
		}
		else
		{
			Agent.ClearNodes();
			Destroy(gameObject);
			MessageHub.Instance.Publish(new EnemyDiedEvent(null));
		}
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