using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	[SerializeField]
	private bool DrawGizmos = false;

	private PathfindingNode[,] _nodes;
	private LinkedList<PathfindingNode> _path = new LinkedList<PathfindingNode>();

	public void RegisterMap(Tile[,] map)
	{
		CreateNodes(map);
	}
	public LinkedList<PathfindingNode> GetPath(Vector2 from, Vector2 to)
	{
		var startNode = _nodes[Mathf.RoundToInt(from.x), Mathf.RoundToInt(from.y)].Copy();
		var endNode = _nodes[Mathf.RoundToInt(to.x), Mathf.RoundToInt(to.y)].Copy();

		return GetPath(startNode, endNode);
	}
	public LinkedList<PathfindingNode> GetPath(PathfindingNode startNode, PathfindingNode endNode)
	{
		if (!startNode.Walkable || !endNode.Walkable)
		{
			_path.Clear();
			return _path;
		}

		var open = new List<PathfindingNode>();
		var closed = new List<PathfindingNode>();

		open.Add(startNode);
		while (open.Count > 0)
		{
			var current = open[0];
			for (var i = 0; i < open.Count; i++)
			{
				var openTile = open[i];
				if (openTile.FCost < current.FCost || openTile.FCost == current.FCost && openTile.HCost < current.HCost)
				{
					current = openTile;
				}
			}
			open.Remove(current);
			closed.Add(current);

			if (current.Equals(endNode))
			{
				break;
			}

			var neighbours = GetNeighbours(current);
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


	private void CreateNodes(Tile[,] tiles)
	{
		_nodes = new PathfindingNode[tiles.GetLength(0) * 2, tiles.GetLength(1) * 2];

		for (var x = 0; x < tiles.GetLength(0); x++)
		{
			for (var y = 0; y < tiles.GetLength(1); y++)
			{
				var xIndex = x * 2;
				var yIndex = y * 2;

				_nodes[xIndex, yIndex] = new PathfindingNode(x + 0.25f, y + 0.25f, false);
				_nodes[xIndex + 1, yIndex] = new PathfindingNode(x + 0.75f, y + 0.25f, false);
				_nodes[xIndex, yIndex + 1] = new PathfindingNode(x + 0.25f, y + 0.75f, false);
				_nodes[xIndex + 1, yIndex + 1] = new PathfindingNode(x + 0.75f, y + 0.75f, false);

				var tile = tiles[x, y];
				if (tile.ConfigurationSquare == null)
				{
					continue;
				}

				switch (tile.ConfigurationSquare.Configuration)
				{
					case 7:
						_nodes[xIndex + 1, yIndex].Walkable = true;
						break;
					case 11:
						_nodes[xIndex, yIndex].Walkable = true;
						break;
					case 13:
						_nodes[xIndex, yIndex + 1].Walkable = true;
						break;
					case 14:
						_nodes[xIndex + 1, yIndex + 1].Walkable = true;
						break;
					case 15:
						_nodes[xIndex, yIndex].Walkable = true;
						_nodes[xIndex + 1, yIndex].Walkable = true;
						_nodes[xIndex, yIndex + 1].Walkable = true;
						_nodes[xIndex + 1, yIndex + 1].Walkable = true;
						break;
					default:
						break;
				}
			}
		}
	}
	private float GetDistance(PathfindingNode from, PathfindingNode to)
	{
		var distanceX = Mathf.Abs(from.X - to.X);
		var distanceY = Mathf.Abs(from.Y - to.Y);

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

				var neighbourX = (int)node.X + x;
				var neighbourY = (int)node.Y + y;
				if (neighbourX < 0 || neighbourX >= _nodes.GetLength(0) || neighbourY < 0 || neighbourY >= _nodes.GetLength(1))
				{
					continue;
				}

				var actualNode = _nodes[neighbourX, neighbourY];
				neighbours.Add(new PathfindingNode(actualNode.X, actualNode.Y, actualNode.Walkable));
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
				Gizmos.color = tile.Walkable ? Color.green : Color.red;
				Gizmos.DrawCube(new Vector3(tile.X, 0.5f, tile.Y), Vector3.one * 0.1f);
			}
		}
	}
}