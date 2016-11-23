using UnityEngine;

public class Player : Character
{
	private Canvas _canvas;
	private RectTransform _bar;

	private AttackData _attackData;
	private MoveData _moveData;
	private Transform _cachedTransform;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	private void Awake()
	{
		_cachedTransform = transform;
		_canvas = transform.FindChild("Canvas").GetComponent<Canvas>();
		_bar = _canvas.transform.FindChild("CurrentHitpoints").GetComponent<RectTransform>();
	}
	private void Update()
	{
		HandleInput();
		_canvas.transform.LookAt(Camera.main.transform.position);
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = InputHandler.Instance.GetRaycastHit();
			if (hit.transform)
			{
				if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
				{
					Move(hit.point);
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (_mouseDragTimer > _mouseDragUpdateInterval)
			{
				_mouseDragTimer = 0f;

				var hit = InputHandler.Instance.GetRaycastHit();
				if (hit.transform)
				{
					if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
					{
						Move(hit.point);
					}
				}
			}
			_mouseDragTimer += Time.deltaTime;
		}

		if (Input.GetMouseButtonDown(1))
		{
			var hit = InputHandler.Instance.GetRaycastHit();
			if (hit.transform != null)
			{
				var adjustedHitPoint = new Vector3(hit.point.x, _cachedTransform.position.y, hit.point.z);
				var direction = (adjustedHitPoint - _cachedTransform.position).normalized;
				Attack(direction);
			}
		}
	}
	private void Move(Vector3 position)
	{
		Agent.StartPathTo(position, _moveData.MovementSpeed, () =>
		{
		});
	}
	public void Attack(Vector3 direction)
	{
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(_attackData.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, _attackData.ProjectileSpeed);
	}

	public override void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth > 0)
		{
			var fraction = HealthData.CurrentHealth / HealthData.TotalHealth;
			_bar.sizeDelta = new Vector2(fraction * 4, _bar.sizeDelta.y);
		}
	}
	public override void SetHealthData(HealthData healthData)
	{
		HealthData = healthData;
	}
	public override void SetAttackData(AttackData attackData)
	{
		_attackData = attackData;
	}
	public override void SetMoveData(MoveData moveData)
	{
		_moveData = moveData;
	}
}