using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	[SerializeField]
	private bool DrawGizmos = false;

	private PathfindingNode[,] _nodes;
	private LinkedList<PathfindingNode> _path = new LinkedList<PathfindingNode>();
	private int _tileSize;

	public void RegisterMap(Tile[,] map, int tileSize)
	{
		_tileSize = tileSize;
		CreateNodes(map);
		GetNeighbours();
	}
	public LinkedList<PathfindingNode> GetPath(Vector2 from, Vector2 to)
	{
		var startNode = GetNode(from);
		var endNode = GetNode(to);

		return GetPath(startNode, endNode);
	}
	public LinkedList<PathfindingNode> GetPath(PathfindingNode startNode, PathfindingNode endNode)
	{
		if (!startNode.Walkable || !endNode.Walkable)
		{
			_path.Clear();
			return _path;
		}

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
				if (!neighbour.Walkable || closed.Contains(neighbour))
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

		return RetracePath(closed.Last());
	}

	private PathfindingNode GetNode(Vector2 worldPosition)
	{
		worldPosition /= _tileSize;
		var fromXFraction = worldPosition.x - (int)worldPosition.x;
		var fromXNodeIndex = Mathf.RoundToInt((int)worldPosition.x * 2 + fromXFraction);
		var fromYFraction = worldPosition.y - (int)worldPosition.y;
		var fromYNodeIndex = Mathf.RoundToInt((int)worldPosition.y * 2 + fromYFraction);

		return new PathfindingNode(_nodes[fromXNodeIndex, fromYNodeIndex]);
	}
	private void CreateNodes(Tile[,] tiles)
	{
		_nodes = new PathfindingNode[tiles.GetLength(0) * 2, tiles.GetLength(1) * 2];

		for (var x = 0; x < tiles.GetLength(0); x++)
		{
			for (var y = 0; y < tiles.GetLength(1); y++)
			{
				var xIndex = x * 2;
				var yIndex = y * 2;
				var tile = tiles[x, y];
				var topLeftWalkable = false;
				var topRightWalkable = false;
				var bottomRightWalkable = false;
				var bottomLeftWalkable = false;

				if (tile.ConfigurationSquare != null)
				{
					switch (tile.ConfigurationSquare.Configuration)
					{
						case 13:
							topLeftWalkable = true;
							break;
						case 11:
							bottomLeftWalkable = true;
							break;
						case 7:
							bottomRightWalkable = true;
							break;
						case 14:
							topRightWalkable = true;
							break;
						case 15:
							topLeftWalkable = true;
							topRightWalkable = true;
							bottomRightWalkable = true;
							bottomLeftWalkable = true;
							break;
						default:
							break;
					}
				}

				_nodes[xIndex, yIndex + 1] = new PathfindingNode(xIndex, yIndex + 1, topLeftWalkable, _tileSize);
				_nodes[xIndex + 1, yIndex + 1] = new PathfindingNode(xIndex + 1, yIndex + 1, topRightWalkable, _tileSize);
				_nodes[xIndex + 1, yIndex] = new PathfindingNode(xIndex + 1, yIndex, bottomRightWalkable, _tileSize);
				_nodes[xIndex, yIndex] = new PathfindingNode(xIndex, yIndex, bottomLeftWalkable, _tileSize);
			}
		}
	}
	private void GetNeighbours()
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
		var distanceX = Mathf.Abs(from.WorldCoordinates.X - to.WorldCoordinates.X);
		var distanceY = Mathf.Abs(from.WorldCoordinates.Y - to.WorldCoordinates.Y);

		if (distanceX > distanceY)
		{
			return distanceY * Constants.DiagonalTileWeight + (distanceX - distanceY) * Constants.HorizontalTileWeight;
		}
		return distanceX * Constants.DiagonalTileWeight + (distanceY - distanceX) * Constants.HorizontalTileWeight;
	}
	private List<PathfindingNode> GetNeighbours(PathfindingNode node)
	{
		var neighbours = new List<PathfindingNode>();
		for (var x = -1; x <= 1; x++)
		{
			for (var y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				var neighbourX = (int)node.GridCoordinates.X + x;
				var neighbourY = (int)node.GridCoordinates.Y + y;
				if (neighbourX < 0 || neighbourX >= _nodes.GetLength(0) || neighbourY < 0 || neighbourY >= _nodes.GetLength(1))
				{
					continue;
				}

				neighbours.Add(_nodes[neighbourX, neighbourY]);
			}
		}

		return neighbours;
	}
	private LinkedList<PathfindingNode> RetracePath(PathfindingNode lastNode)
	{
		var path = new LinkedList<PathfindingNode>();
		path.AddFirst(lastNode);

		var parent = lastNode.Parent;
		while (parent != null)
		{
			path.AddFirst(parent);

			parent = parent.Parent;
		}

		return path;
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
				Gizmos.color = tile.Walkable ? Color.blue : Color.yellow;
				Gizmos.DrawCube(new Vector3(tile.WorldCoordinates.X, 0.5f, tile.WorldCoordinates.Y), Vector3.one * 0.1f);
			}
		}
	}
}