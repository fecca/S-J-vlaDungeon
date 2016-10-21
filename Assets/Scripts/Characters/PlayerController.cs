using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class PlayerController : MonoBehaviour
{
	private PathFinderAgent _agent;

	private void Start()
	{
		_agent = GetComponent<PathFinderAgent>();
		_agent.Setup(FindObjectOfType<PathFinder>());
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	public void ClickedGround(Vector2 targetPosition)
	{
		_agent.StartPath(new Vector2(transform.position.x, transform.position.z), targetPosition);
	}
}