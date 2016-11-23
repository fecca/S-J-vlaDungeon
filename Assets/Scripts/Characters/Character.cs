using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public abstract class Character : MonoBehaviour, IDamagable
{
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

	public HealthData HealthData { get; protected set; }

	public void InitializePathfindingAgent()
	{
		var pathFinder = FindObjectOfType<PathFinder>();
		var node = pathFinder.GetRandomWalkableNode();
		transform.position = node.WorldCoordinates + Vector3.up * 5;
		Agent.Setup(pathFinder, node);
	}
	public virtual void SetHealthData(HealthData healthData) { }
	public virtual void SetAttackData(AttackData attackData) { }
	public virtual void SetMoveData(MoveData moveData) { }
	public virtual void SetPerceptionData(PerceptionData perceptionData) { }
	public abstract void TakeDamage();
}