using UnityEngine;

public class Player : Character
{
	private AttackerData _attackerData;
	private MoverData _moverData;
	private Transform _cachedTransform;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	private void Awake()
	{
		_cachedTransform = transform;
		_attackerData = ScriptableObject.CreateInstance<AttackerData>();
		_attackerData.ProjectilePrefab = Resources.Load<GameObject>("PlayerProjectile");
		_moverData = ScriptableObject.CreateInstance<MoverData>();
	}
	private void Update()
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
	private void OnDrawGizmos()
	{
		Debug.DrawRay(_cachedTransform.position, _cachedTransform.forward, Color.red);
	}

	private void Move(Vector3 position)
	{
		Agent.StartPathTo(position, _moverData.MovementSpeed, () =>
		{
		});
	}

	public override void TakeDamage()
	{
	}
	public void Attack(Vector3 direction)
	{
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(_attackerData.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, _attackerData.ProjectileSpeed);
	}
}