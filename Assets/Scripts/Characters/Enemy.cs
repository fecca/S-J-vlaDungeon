using UnityEngine;

public class Enemy : Character, IAttacking, IMoving
{
	[SerializeField]
	private Canvas _canvas;
	private RectTransform _bar;
	private float _totalHitpoints = 5;
	private float _currentHitpoints = 5;

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

	public override void TakeDamage()
	{
		_currentHitpoints--;
		if (_currentHitpoints > 0)
		{
			_bar.sizeDelta = new Vector2(4 * (_currentHitpoints / _totalHitpoints), _bar.sizeDelta.y);
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