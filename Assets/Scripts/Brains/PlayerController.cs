using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float MovementSpeed = 30.0f;

	private PathFinderAgent _agent;

	private void Start()
	{
		_agent = GetComponent<PathFinderAgent>();
		_agent.Setup(FindObjectOfType<PathFinder>());
	}

	public void ClickedGround(Vector3 targetPosition)
	{
		_agent.StartPathTo(targetPosition, MovementSpeed, () =>
		{
			Debug.Log("Arrived at target");
		});
	}
}