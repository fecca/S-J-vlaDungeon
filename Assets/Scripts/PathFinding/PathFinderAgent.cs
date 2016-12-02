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
	private PathfindingNode _currentNode;
	private PathfindingNode _nextNode;
	private Transform _cachedTransform;

	private void Awake()
	{
		_cachedTransform = transform;
	}
	private void FixedUpdate()
	{
		if (_path.Count > 0)
		{
			if (_nextNode == null)
			{
				_nextNode = _path.First.Value;
				if (_nextNode.Occupied)
				{
					var lastNodeWorldCoordinates = _path.Last.Value.WorldCoordinates;
					_path.Clear();
					_nextNode = null;
					_pathFinder.GetPath(_cachedTransform.position, lastNodeWorldCoordinates, (path) =>
					{
						_path = path;
					});
					return;
				}
				else
				{
					_nextNode.SetOccupied(true);
				}
				var targetPosition = _nextNode.WorldCoordinates.WithY(_cachedTransform.position.y);
				RotateAgent(_cachedTransform.GetDirectionTo(targetPosition));
			}
			MoveAlongPath();
		}
	}
	private void OnDrawGizmos()
	{
		//if (_path != null)
		//{
		//	for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
		//	{
		//		Gizmos.color = Color.blue;
		//		Gizmos.DrawSphere(iteration.Value.WorldCoordinates.WithY(1), 0.25f);
		//	}
		//}
	}

	public void Setup(PathFinder pathFinder, PathfindingNode startingNode)
	{
		_pathFinder = pathFinder;
		_currentNode = startingNode;
		_currentNode.SetOccupied(true);
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
			_pathFinder.GetPath(_cachedTransform.position, targetPosition, (path) =>
			{
				_path = path;
			});
		}
	}
	public void StartPathTo(Tile targetTile, float movementSpeed, Action completed = null)
	{
		var position = targetTile.WorldCoordinates.WithY(_cachedTransform.position.y);
		StartPathTo(position, movementSpeed, completed);
	}
	public void SmoothStop()
	{
		var firstNode = _path.First;
		_path.Clear();
		if (firstNode != null)
		{
			_path.AddFirst(firstNode);
		}
	}
	public void ClearNodes()
	{
		if (_currentNode != null)
		{
			_currentNode.SetOccupied(false);
		}
		if (_nextNode != null)
		{
			_nextNode.SetOccupied(false);
		}
	}
	public void RotateAgent(Vector3 direction)
	{
		_cachedTransform.rotation = Quaternion.LookRotation(direction);
	}

	private void MoveAlongPath()
	{
		var targetPosition = _nextNode.WorldCoordinates.WithY(_cachedTransform.position.y);
		_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, targetPosition, Time.deltaTime * _movementSpeed);

		if (Vector3.Distance(_cachedTransform.position, targetPosition) < 0.1f)
		{
			ArrivedAtNode();
		}
	}
	private void ArrivedAtNode()
	{
		_currentNode.SetOccupied(false);
		_currentNode = _nextNode;
		_nextNode = null;

		if (_pendingNewPath)
		{
			_pendingNewPath = false;
			_pathFinder.GetPath(_cachedTransform.position, _pendingTo, (path) =>
			{
				_path = path;
			});
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