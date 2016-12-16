using UnityEngine;

public class Player : MonoBehaviour, ICharacter, IMover, IAttacker
{
	private AttackData _attackData;
	private MoveData _moveData;
	private Transform _cachedTransform;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;
	private PathFinderAgent _agent;

	public HealthData HealthData { get; set; }
	public PathFinderAgent Agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = GetComponent<PathFinderAgent>();
			}
			return _agent;
		}
	}

	private void Awake()
	{
		ServiceLocator<ICharacter>.Instance = this;
		_cachedTransform = transform;
	}
	private void Update()
	{
		HandleInput();
	}

	public void InitializePathfindingAgent()
	{
		var pathFinder = ServiceLocator<IPathFinder>.Instance;
		var node = pathFinder.GetRandomWalkableNode();
		transform.position = node.WorldCoordinates + Vector3.up * 5;
		Agent.Setup(pathFinder, node);
	}
	public void InitializeHealth(HealthData healthData)
	{
		HealthData = healthData;
	}
	public void InitializeAttacker(AttackData attackData)
	{
		_attackData = attackData;
	}
	public void InitializeMover(MoveData moveData)
	{
		_moveData = moveData;
	}
	public void InitializerPerception(PerceptionData perceptionData)
	{
	}

	public void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth <= 0)
		{
			Destroy(gameObject);
		}
	}
	public void Attack(AttackData data, Vector3 direction)
	{
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(_attackData.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, _attackData.ProjectileSpeed);
	}
	public void Move(MoveData data, Vector3 targetPosition)
	{
		Agent.StartPathTo(targetPosition, _moveData.MovementSpeed, () =>
		{
		});
	}
	public void SmoothStop()
	{
		Agent.SmoothStop();
	}
	public Vector3 GetTransformPosition()
	{
		return _cachedTransform.position;
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
					Move(_moveData, hit.point);
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
						Move(_moveData, hit.point);
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
				var hitPosition = hit.point.WithY(_cachedTransform.position.y);
				var direction = _cachedTransform.GetDirectionTo(hitPosition);
				Attack(_attackData, direction);
			}
		}
	}
}