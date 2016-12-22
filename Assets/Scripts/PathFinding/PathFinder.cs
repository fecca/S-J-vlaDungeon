using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour, IPathFinder
{
	[SerializeField]
	private bool DrawGizmos = false;

	private PathfindingNode[,] _nodes;
	private List<PathfindingNode> _walkableNodes;

	private void Awake()
	{
		ServiceLocator<IPathFinder>.Instance = this;
		MessageHub.Instance.Subscribe<MeshCreatedEvent>(OnMeshCreatedEvent);
		MessageHub.Instance.Subscribe<CharactersDestroyedEvent>(OnCharactersDestroyedEvent);
	}
	private void OnDrawGizmos()
	{
		if (!DrawGizmos)
		{
			return;
		}

		if (_nodes != null)
		{
			foreach (var tile in _nodes)
			{
				Gizmos.color = tile.Occupied ? Color.magenta : tile.Walkable ? Color.green : Color.red;
				Gizmos.DrawCube(tile.WorldCoordinates, Vector3.one * 0.5f);
			}
		}
	}

	private void OnMeshCreatedEvent(MeshCreatedEvent meshCreatedEvent)
	{
		StartCoroutine(CreatePathNodes(meshCreatedEvent.Map, () =>
		{
			MessageHub.Instance.Publish(new PathNodesCreatedEvent(null));
		}));
	}
	private void OnCharactersDestroyedEvent(CharactersDestroyedEvent charactersDestroyedEvent)
	{
		_nodes = null;
		_walkableNodes.Clear();

		MessageHub.Instance.Publish(new PathNodesDestroyedEvent(null));
	}

	public void GetPath(Vector3 from, Vector3 to, Action<LinkedList<PathfindingNode>> completed)
	{
		var startNode = GetNodeCopy(from);
		var endNode = GetNodeCopy(to);

		if (!startNode.Walkable || !endNode.Walkable)
		{
			completed(new LinkedList<PathfindingNode>());
			return;
		}

		StartCoroutine(GetPath(startNode, endNode, (lastNode) =>
		{
			completed(RevertPath(lastNode));
		}));
	}
	public PathfindingNode GetNodeCopy(Vector3 worldPosition)
	{
		worldPosition /= Constants.TileSize;
		var fromXFraction = worldPosition.x - (int)worldPosition.x;
		var fromXNodeIndex = Mathf.RoundToInt((int)worldPosition.x * 2 + fromXFraction);
		var fromYFraction = worldPosition.z - (int)worldPosition.z;
		var fromYNodeIndex = Mathf.RoundToInt((int)worldPosition.z * 2 + fromYFraction);
		var node = _nodes[fromXNodeIndex, fromYNodeIndex];

		return new PathfindingNode(node);
	}
	public PathfindingNode GetRandomWalkableNode()
	{
		return _walkableNodes.GetRandomElement();
	}
	public List<PathfindingNode> GetAvailableNeighouringNodes(Vector3 worldPosition)
	{
		var currentNode = GetNodeCopy(worldPosition);
		var neighbours = GetNeighbours(currentNode);
		neighbours.Add(currentNode);

		return neighbours.Where(p => !p.Occupied).ToList();
	}

	private IEnumerator CreatePathNodes(Tile[,] map, Action completed)
	{
		CreateNodes(map);
		SetNeighbours();

		completed();

		yield break;
	}
	private void CreateNodes(Tile[,] tiles)
	{
		_nodes = new PathfindingNode[tiles.GetLength(0) * 2, tiles.GetLength(1) * 2];
		_walkableNodes = new List<PathfindingNode>(_nodes.Length);
		for (var x = 0; x < tiles.GetLength(0); x++)
		{
			for (var y = 0; y < tiles.GetLength(1); y++)
			{
				var xIndex = x * 2;
				var yIndex = y * 2;
				var tile = tiles[x, y];
				var topLeftNode = new PathfindingNode(xIndex, yIndex + 1);
				var topRightNode = new PathfindingNode(xIndex + 1, yIndex + 1);
				var bottomRightNode = new PathfindingNode(xIndex + 1, yIndex);
				var bottomLeftNode = new PathfindingNode(xIndex, yIndex);

				_nodes[xIndex, yIndex + 1] = topLeftNode;
				_nodes[xIndex + 1, yIndex + 1] = topRightNode;
				_nodes[xIndex + 1, yIndex] = bottomRightNode;
				_nodes[xIndex, yIndex] = bottomLeftNode;

				if (tile.Type == TileType.Ground)
				{
					topLeftNode.Walkable = true;
					_walkableNodes.Add(topLeftNode);

					topRightNode.Walkable = true;
					_walkableNodes.Add(topRightNode);

					bottomRightNode.Walkable = true;
					_walkableNodes.Add(bottomRightNode);

					bottomLeftNode.Walkable = true;
					_walkableNodes.Add(bottomLeftNode);
				}
			}
		}
	}
	private void SetNeighbours()
	{
		for (int x = 0; x < _nodes.GetLength(0); x++)
		{
			for (int y = 0; y < _nodes.GetLength(1); y++)
			{
				var node = _nodes[x, y];
				node.Neighbours = GetNeighbours(node);
			}
		}
	}
	private float GetDistance(PathfindingNode from, PathfindingNode to)
	{
		var distanceX = Mathf.Abs(from.WorldCoordinates.x - to.WorldCoordinates.x);
		var distanceY = Mathf.Abs(from.WorldCoordinates.z - to.WorldCoordinates.z);

		if (distanceX > distanceY)
		{
			return distanceY * Constants.DiagonalTileWeight + (distanceX - distanceY) * Constants.HorizontalTileWeight;
		}
		return distanceX * Constants.DiagonalTileWeight + (distanceY - distanceX) * Constants.HorizontalTileWeight;
	}
	private List<PathfindingNode> GetNeighbours(PathfindingNode node)
	{
		var neighbours = new List<PathfindingNode>(8);
		for (var x = -1; x <= 1; x++)
		{
			for (var y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				var neighbourX = node.GridCoordinates.X + x;
				var neighbourY = node.GridCoordinates.Y + y;
				if (neighbourX < 0 || neighbourX >= _nodes.GetLength(0) || neighbourY < 0 || neighbourY >= _nodes.GetLength(1))
				{
					continue;
				}

				neighbours.Add(_nodes[neighbourX, neighbourY]);
			}
		}

		return neighbours;
	}
	private LinkedList<PathfindingNode> RevertPath(PathfindingNode lastNode)
	{
		var path = new LinkedList<PathfindingNode>();
		if (lastNode == null)
		{
			return path;
		}
		path.AddFirst(lastNode);

		var parent = lastNode.Parent;
		while (parent != null)
		{
			path.AddFirst(parent);
			parent = parent.Parent;
		}

		path.RemoveFirst();
		RemoveOccupiedEndings(path);

		return path;
	}
	private void RemoveOccupiedEndings(LinkedList<PathfindingNode> path)
	{
		while (path.Count > 0 && path.First.Value.Occupied)
		{
			path.RemoveFirst();
		}
	}
	private IEnumerator GetPath(PathfindingNode startNode, PathfindingNode endNode, Action<PathfindingNode> completed)
	{
		var open = new Heap<PathfindingNode>(_nodes.GetLength(0) * _nodes.GetLength(1));
		var closed = new HashSet<PathfindingNode>();

		open.Add(startNode);
		while (open.Count > 0)
		{
			var current = open.RemoveFirst();
			closed.Add(current);

			if (current.Equals(endNode))
			{
				break;
			}

			var neighbours = current.Neighbours;
			for (var i = 0; i < neighbours.Count; i++)
			{
				var neighbour = neighbours[i];
				if ((neighbour.Occupied && !neighbour.Equals(endNode)) || !neighbour.Walkable || closed.Contains(neighbour))
				{
					continue;
				}

				var newMovementCostToNeighbour = current.GCost + GetDistance(current, neighbour);
				if (newMovementCostToNeighbour < neighbour.GCost || !open.Contains(neighbour))
				{
					var distance = GetDistance(neighbour, endNode);
					neighbour.GCost = newMovementCostToNeighbour;
					neighbour.HCost = distance;
					neighbour.FCost = newMovementCostToNeighbour + distance;
					neighbour.Parent = current;

					if (!open.Contains(neighbour))
					{
						open.Add(neighbour);
					}
				}
			}
		}

		completed(closed.Last());

		yield break;
	}
}