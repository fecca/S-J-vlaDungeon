using UnityEngine;

public class Player : Character
{
	[SerializeField]
	private Canvas _canvas = null;
	private RectTransform _bar;

	private AttackerData _attackerData;
	private MoverData _moverData;
	private Transform _cachedTransform;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	private void Awake()
	{
		_cachedTransform = transform;
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
		Agent.StartPathTo(position, _moverData.MovementSpeed, () =>
		{
		});
	}

	public override void SetHealth(HealthData healthData)
	{
		Stats = new Stats(healthData);
	}
	public void SetToAttacker(AttackerData attackerData)
	{
		_attackerData = attackerData;
	}
	public void SetToMover(MoverData moverData)
	{
		_moverData = moverData;
	}
	public override void TakeDamage()
	{
		Stats.CurrentHealth--;
		if (Stats.CurrentHealth > 0)
		{
			var fraction = Stats.CurrentHealth / Stats.TotalHitpoints;
			_bar.sizeDelta = new Vector2(fraction * 4, _bar.sizeDelta.y);
		}
	}
	public void Attack(Vector3 direction)
	{
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(_attackerData.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, _attackerData.ProjectileSpeed);
	}
}