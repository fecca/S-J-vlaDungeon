using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public abstract class Character : MonoBehaviour
{
	private PathFinderAgent _agent;
	public PathFinderAgent Agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = GetComponent<PathFinderAgent>();
				_agent.Setup(FindObjectOfType<PathFinder>());
			}
			return _agent;
		}
	}
}