using UnityEngine;

public class Player : MonoBehaviour, ICharacter, IMover, IAttacker
{
	private Transform _cachedTransform;
	private PlayerBrain _brain;
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
		if (_brain != null)
		{
			_brain.Think();
		}
	}

	public void InitializeBrain()
	{
		_brain = new PlayerBrain(this);
	}
	public void InitializePathfindingAgent()
	{
		Agent.Initialize();
	}
	public void InitializeHealth(HealthData healthData)
	{
		HealthData = healthData;
	}
	public void InitializeAttacker(AttackData attackData)
	{
		_brain.InitializeAttacker(attackData, this);
	}
	public void InitializeMover(MoveData moveData)
	{
		_brain.InitializeMover(moveData, this);
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

		var projectile = Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, data.ProjectileSpeed);
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
	public GameObject GetGameObject()
	{
		return gameObject;
	}
	public Vector3 GetTransformPosition()
	{
		return _cachedTransform.position;
	}
}