using System;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter, IMover, IAttacker, IPerceiver
{
	private Transform _cachedTransform;
	private Brain _brain;
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
		_cachedTransform = transform;
	}
	private void Update()
	{
		CheckPerception();
		if (_brain != null)
		{
			_brain.Think();
		}
	}

	public void InitializeBrain()
	{
		_brain = new Brain(this);
	}
	public void InitializePathfindingAgent()
	{
		var pathFinder = FindObjectOfType<PathFinder>();
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
		_brain.InitializeAttacker(attackData, this);
	}
	public void InitializeMover(MoveData moveData)
	{
		_brain.InitializeMover(moveData, this);
	}
	public void InitializerPerception(PerceptionData perceptionData)
	{
		_brain.InitializePerception(perceptionData);
	}

	public void SetTarget(ICharacter target)
	{
		_brain.SetTarget(target);
	}
	public void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth <= 0)
		{
			Agent.ClearNodes();
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
	public Vector3 GetTransformPosition()
	{
		return _cachedTransform.position;
	}
	public void CheckPerception()
	{
		_brain.Perceive();
	}
}