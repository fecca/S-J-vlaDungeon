using System.Collections.Generic;
using UnityEngine;

public class PathFinderAgent : MonoBehaviour
{
	[SerializeField]
	private float MovementSpeed = 5.0f;

	private LinkedList<PathfindingNode> _path;
	private PathFinder _pathFinder;
	private bool _pendingNewPath;
	private Vector2 _pendingTo;

	public void Setup(PathFinder pathFinder)
	{
		_path = new LinkedList<PathfindingNode>();
		_pathFinder = pathFinder;
	}

	public void StartPath(Vector2 from, Vector2 to)
	{
		if (_path.Count > 0)
		{
			_pendingNewPath = true;
			_pendingTo = to;
		}
		else
		{
			_pendingNewPath = false;
			_path = _pathFinder.GetPath(from, to);
		}
	}

	private void MoveAlongPath()
	{
		var targetNode = _path.First;

		var targetPosition = new Vector3(targetNode.Value.WorldCoordinates.X, transform.position.y, targetNode.Value.WorldCoordinates.Y);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * MovementSpeed);

		if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
		{
			_path.Remove(targetNode);
			if (_pendingNewPath)
			{
				_pendingNewPath = false;
				_path = _pathFinder.GetPath(new Vector2(transform.position.x, transform.position.z), _pendingTo);
			}
		}
	}

	private void FixedUpdate()
	{
		if (_path.Count > 0)
		{
			MoveAlongPath();
		}
	}
	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawCube(new Vector3(iteration.Value.WorldCoordinates.X, 1, iteration.Value.WorldCoordinates.Y), Vector3.one * 0.25f);
			}
		}
	}
}