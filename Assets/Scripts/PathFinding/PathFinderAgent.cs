using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderAgent : MonoBehaviour, IPathFinderAgent
{
	private LinkedList<PathfindingNode> _path = new LinkedList<PathfindingNode>();
	private IPathFinder _pathFinder;
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
	private void Start()
	{
		_pathFinder = ServiceLocator<IPathFinder>.Instance;
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
					_nextNode.Occupied = true;
				}
				var targetPosition = _nextNode.WorldCoordinates.WithY(_cachedTransform.position.y);
				RotateAgent(_cachedTransform.GetDirectionTo(targetPosition));
			}
			MoveAlongPath();
		}
	}

	public void Initialize()
	{
		_currentNode = ServiceLocator<IPathFinder>.Instance.GetRandomWalkableNode();
		_currentNode.Occupied = true;

		_cachedTransform.position = _currentNode.WorldCoordinates + Vector3.up * 5;
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
	public void SmoothStop()
	{
		var firstNode = _path.First;
		_path.Clear();
		if (firstNode != null)
		{
			_path.AddFirst(firstNode);
		}
	}
	public void ClearOccupiedNodes()
	{
		if (_currentNode != null)
		{
			_currentNode.Occupied = false;
		}
		if (_nextNode != null)
		{
			_nextNode.Occupied = false;
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
		_currentNode.Occupied = false;
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