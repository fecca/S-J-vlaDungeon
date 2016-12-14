using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public abstract class Character : MonoBehaviour, ICharacter
{
	public HealthData HealthData { get; protected set; }
	private PathFinderAgent _agent;
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
	public abstract void InitializePathfindingAgent();
	public abstract void SetHealthData(HealthData healthData);
	public abstract void SetAttackData(AttackData attackData);
	public abstract void SetMoveData(MoveData moveData);
	public abstract void SetPerceptionData(PerceptionData perceptionData);
	public abstract void TakeDamage();
	public abstract Vector3 GetTransformPosition();
	public abstract void Attack(AttackData data, Vector3 direction);
	public abstract void Move(MoveData data, Vector3 targetPosition);
}