using UnityEngine;

public class Enemy : Character, IAttacking, IMoving
{
	private Canvas _canvas;
	private RectTransform _bar;

	private Transform _cachedTransform;
	private Brain _brain;
	private Transform _target;

	private void Awake()
	{
		_cachedTransform = transform;
		_canvas = transform.FindChild("Canvas").GetComponent<Canvas>();
		_bar = _canvas.transform.FindChild("CurrentHitpoints").GetComponent<RectTransform>();
	}
	private void Update()
	{
		if (_brain != null)
		{
			_brain.Think();
		}
		_canvas.transform.LookAt(Camera.main.transform.position);
	}

	public override void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth > 0)
		{
			var fraction = HealthData.CurrentHealth / HealthData.TotalHealth;
			_bar.sizeDelta = new Vector2(fraction * 4, _bar.sizeDelta.y);
		}
		else
		{
			Agent.ClearNodes();
			Destroy(gameObject);
			MessageHub.Instance.Publish(new EnemyDiedEvent(null));
		}
	}
	public void Attack(AttackData data)
	{
		var targetPosition = _target.position;
		targetPosition.y = _cachedTransform.position.y;
		var direction = (targetPosition - _cachedTransform.position).normalized;
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, data.ProjectileSpeed);
	}
	public void Move(MoveData data)
	{
		Agent.StartPathTo(_target.position, data.MovementSpeed, () =>
		{
		});
	}

	public override void SetData(HealthData healthData, AttackData attackData, MoveData moveData, PerceptionData perceptionData)
	{
		HealthData = healthData;

		_target = FindObjectOfType<Player>().transform;
		_brain = new Brain(this, _target);
		_brain.SetToAttacker(attackData);
		_brain.SetToMover(moveData);
		_brain.SetToPerceiver(perceptionData);
	}
}