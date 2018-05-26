using UnityEngine;

public class PlayerBrain : IBrain, IAttacker, IMover
{
	private Player _owner;
	private IPathFinderAgent _agent;
	private AttackData _attackData;
	private MoveData _moveData;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	public IPathFinderAgent Agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = _owner.GetGameObject().GetComponent<PathFinderAgent>();
			}
			return _agent;
		}
	}

	public PlayerBrain(Player owner)
	{
		_owner = owner;
	}

	public void InitializePathfindingAgent()
	{
		Agent.Initialize();
	}
	public void InitializeAttacker(AttackData attackData)
	{
		if (attackData != null)
		{
			_attackData = attackData;
		}
	}
	public void InitializeMover(MoveData moveData)
	{
		if (moveData != null)
		{
			_moveData = moveData;
		}
	}
	public void Think()
	{
		HandleInput();
	}
	public void Attack(AttackData data, Vector3 direction)
	{
		//Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = UnityEngine.Object.Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_owner.GetTransformPosition(), direction, data.ProjectileSpeed);
	}
	public void Move(MoveData data, Vector3 targetPosition)
	{
		Agent.StartPathTo(targetPosition, data.MovementSpeed, () =>
		{
		});
	}
	public void SmoothStop()
	{
		Agent.SmoothStop();
	}
	public void ClearOccupiedAgentNodes()
	{
		Agent.ClearOccupiedNodes();
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
			if (hit.transform != null)
			{
				if (hit.transform.CompareTag("Ground"))
				{
					Move(_moveData, hit.point);
					return;
				}

				if (hit.transform.CompareTag("Item"))
				{
					var itemContainer = hit.transform.GetComponent<ItemContainer>();
					_owner.AddItemToInventory(itemContainer.Item);
					Object.Destroy(itemContainer.gameObject);
					return;
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (_mouseDragTimer > _mouseDragUpdateInterval)
			{
				_mouseDragTimer = 0f;

				var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
				if (hit.transform != null)
				{
					if (hit.transform.CompareTag("Ground"))
					{
						Move(_moveData, hit.point);
					}
				}
			}
			_mouseDragTimer += Time.deltaTime;
		}

		if (Input.GetMouseButtonDown(1))
		{
			var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
			if (hit.transform != null)
			{
				var targetPosition = hit.point.WithY(_owner.GetTransformPosition().y);
				var direction = _owner.GetTransformPosition().GetDirectionTo(targetPosition);
				Attack(_attackData, direction);
			}
		}
	}
}