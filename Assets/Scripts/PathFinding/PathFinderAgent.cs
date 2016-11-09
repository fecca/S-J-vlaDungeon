using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderAgent : MonoBehaviour
{
	private LinkedList<PathfindingNode> _path = new LinkedList<PathfindingNode>();
	private PathFinder _pathFinder;
	private Vector3 _pendingTo;
	private Action _completedPath;
	private bool _pendingNewPath;
	private float _movementSpeed;

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
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(new Vector3(iteration.Value.WorldCoordinates.x, 1, iteration.Value.WorldCoordinates.z), 0.25f);
			}
		}
	}

	public void Setup(PathFinder pathFinder)
	{
		_pathFinder = pathFinder;
	}
	public void StartPathTo(Vector3 targetPosition, float movementSpeed, Action completed = null)
	{
		_movementSpeed = movementSpeed;
		_completedPath = completed;
		if (_path.Count > 0)
		{
			_pendingNewPath = true;
			_pendingTo = targetPosition;
		}
		else
		{
			_pendingNewPath = false;
			_path = _pathFinder.GetPath(transform.position, targetPosition);
		}
	}
	public void StartPathTo(Tile targetTile, float movementSpeed, Action completed = null)
	{
		StartPathTo(new Vector3(targetTile.WorldCoordinates.x, transform.position.y, targetTile.WorldCoordinates.z), movementSpeed, completed);
	}
	public void Stop()
	{
		_path.Clear();
	}

	private void MoveAlongPath()
	{
		var targetNode = _path.First;
		var targetPosition = new Vector3(targetNode.Value.WorldCoordinates.x, transform.position.y, targetNode.Value.WorldCoordinates.z);
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * _movementSpeed);

		if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
		{
			ArrivedAtNode();
		}
	}
	private void ArrivedAtNode()
	{
		if (_pendingNewPath)
		{
			_pendingNewPath = false;
			_path = _pathFinder.GetPath(transform.position, _pendingTo);
		}
		else
		{
			_path.RemoveFirst();
			if (_path.Count == 0)
			{
				if (_completedPath != null)
				{
					_completedPath();
				}
			}
		}
	}
}