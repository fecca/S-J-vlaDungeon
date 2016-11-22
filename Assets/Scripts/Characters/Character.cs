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

	public void Initialize(PathFinder pathFinder)
	{
		var node = pathFinder.GetRandomWalkableNode();
		transform.position = node.WorldCoordinates + Vector3.up * 5;
		Agent.Setup(pathFinder, node);
	}
	public abstract void SetData(HealthData healthData, AttackData attackData, MoveData moveData, PerceptionData perceptionData);
	public abstract void TakeDamage();
}