using Pathfinding;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	//The point to move to
	public Vector3 targetPosition;
	private Seeker seeker;
	private CharacterController controller;
	//The calculated path
	public Path path;
	//The AI's speed per second
	public float movementSpeed = 1000;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	public void Start()
	{
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
	}
	public void SetPosition(Vector3 position)
	{
		position.y = 1;
		transform.position = position;
	}
	public void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	public void Update()
	{
		if (Input.GetMouseButtonUp(1))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f))
			{
				path = null;
				seeker.StartPath(transform.position, hit.point, OnPathComplete);
			}
		}

		if (path == null)
		{
			//We have no path to move after yet
			return;
		}
		if (currentWaypoint >= path.vectorPath.Count)
		{
			return;
		}

		//Direction to the next waypoint
		var direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		direction *= movementSpeed * Time.deltaTime;
		controller.SimpleMove(direction);

		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
		{
			currentWaypoint++;
		}
	}
}