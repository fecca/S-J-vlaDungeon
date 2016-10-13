using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class Enemy : MonoBehaviour
{
	private float _timer;
	private const float Time = 5f;
	private PathFinderAgent _agent;

	private void Start()
	{
		_agent = GetComponent<PathFinderAgent>();
		_agent.Setup(FindObjectOfType<PathFinder>());
	}

	private void Update()
	{
		if (_timer > Time)
		{
			_timer = 0;
			return;
		}

		//_agent.StartPath(transform.position);
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}
}