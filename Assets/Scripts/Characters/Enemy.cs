using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
{
	private Transform _cachedTransform;
	private EnemyBrain _brain;

	public HealthData HealthData { get; set; }

	private void Awake()
	{
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
		_brain = new EnemyBrain(this);
	}
	public void InitializePathfindingAgent()
	{
		_brain.InitializePathfindingAgent();
	}
	public void InitializeHealth(HealthData healthData)
	{
		HealthData = healthData;
	}
	public void InitializeAttacker(AttackData attackData)
	{
		_brain.InitializeAttacker(attackData);
	}
	public void InitializeMover(MoveData moveData)
	{
		_brain.InitializeMover(moveData);
	}
	public void InitializePerception(PerceptionData perceptionData)
	{
		_brain.InitializePerception(perceptionData);
	}
	public void InitializeTarget(ICharacter target)
	{
		_brain.InitializeTarget(target);
	}

	public void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth <= 0)
		{
			_brain.ClearOccupiedAgentNodes();
			Destroy(gameObject);
		}
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