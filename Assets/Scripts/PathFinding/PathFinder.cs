using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	private PathfindingNode[,] _map;
	private LinkedList<PathfindingNode> _path = new LinkedList<PathfindingNode>();

	public void RegisterMap(Tile[,] map)
	{
		CreateNodes(map);
	}
	public LinkedList<PathfindingNode> GetPath(Vector2 from, Vector2 to)
	{
		var startNode = _map[Mathf.RoundToInt(from.x), Mathf.RoundToInt(from.y)].Copy();
		var endNode = _map[Mathf.RoundToInt(to.x), Mathf.RoundToInt(to.y)].Copy();

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
		_map = new PathfindingNode[tiles.GetLength(0) * Constants.TileSize, tiles.GetLength(1) * Constants.TileSize];

		for (var x = 0; x < tiles.GetLength(0); x++)
		{
			for (var y = 0; y < tiles.GetLength(1); y++)
			{
				for (var tileSizeX = 1; tileSizeX < Constants.TileSize + 1; tileSizeX++)
				{
					for (var tileSizeY = 1; tileSizeY < Constants.TileSize + 1; tileSizeY++)
					{
						var xIndex = x * Constants.TileSize + (tileSizeX - 1);
						var yIndex = y * Constants.TileSize + (tileSizeY - 1);
						_map[xIndex, yIndex] = new PathfindingNode(xIndex, yIndex, tiles[x, y].IsWalkable);
					}
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
				if (neighbourX < 0 || neighbourX >= _map.GetLength(0) || neighbourY < 0 || neighbourY >= _map.GetLength(1))
				{
					continue;
				}

				var actualNode = _map[neighbourX, neighbourY];
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
		if (_map != null)
		{
			foreach (var tile in _map)
			{
				Gizmos.color = tile.Walkable ? Color.green : Color.red;
				Gizmos.DrawCube(new Vector3(tile.X, 0.5f, tile.Y), Vector3.one * 0.25f);
			}
		}
	}
}