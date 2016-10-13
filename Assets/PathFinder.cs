using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	private Tile[,] _map;
	private LinkedList<PathNode> _path = new LinkedList<PathNode>();

	public void RegisterMap(Tile[,] map)
	{
		_map = map;

		//_path = GetPath(_map[11, 8], _map[10, 17]);
	}
	public LinkedList<PathNode> GetPath(Vector2 from, Vector2 to)
	{
		return GetPath(_map[Mathf.RoundToInt(from.x), Mathf.RoundToInt(from.y)], _map[Mathf.RoundToInt(to.x), Mathf.RoundToInt(to.y)]);
	}
	public LinkedList<PathNode> GetPath(Tile from, Tile to)
	{
		var fromNode = new PathNode(from);
		var toNode = new PathNode(to);
		if (!fromNode.IsWalkable || !toNode.IsWalkable)
		{
			_path.Clear();
			return _path;
		}

		var open = new List<PathNode>();
		var closed = new List<PathNode>();

		open.Add(fromNode);
		while (open.Count > 0)
		{
			var current = open[0];
			for (var i = 0; i < open.Count; i++)
			{
				var openTile = open[i];
				if (openTile.FCost < current.FCost
					|| openTile.FCost == current.FCost && openTile.HCost < current.HCost)
				{
					current = openTile;
				}
			}
			open.Remove(current);
			closed.Add(current);

			if (current.Tile == to)
			{
				break;
			}

			var neighbours = GetNeighbours(current);
			for (var i = 0; i < neighbours.Count; i++)
			{
				var neighbour = neighbours[i];
				if (!neighbour.IsWalkable || closed.Contains(neighbour))
				{
					continue;
				}

				var newMovementCostToNeighbour = current.GCost + GetDistance(current, neighbour);
				if (newMovementCostToNeighbour < neighbour.GCost || !open.Contains(neighbour))
				{
					neighbour.GCost = newMovementCostToNeighbour;
					neighbour.HCost = GetDistance(neighbour, toNode);
					neighbour.Parent = current;

					if (!open.Contains(neighbour))
					{
						open.Add(neighbour);
					}
				}
			}
		}

		return RetracePath(closed.First(), closed.Last());
	}

	private int GetDistance(PathNode from, PathNode to)
	{
		var distanceX = Mathf.Abs(from.Tile.Coordinates.X - to.Tile.Coordinates.X);
		var distanceY = Mathf.Abs(from.Tile.Coordinates.Y - to.Tile.Coordinates.Y);

		if (distanceX > distanceY)
		{
			return distanceY * Constants.DiagonalTileWeight + (distanceX - distanceY) * Constants.HorizontalTileWeight;
		}
		return distanceX * Constants.DiagonalTileWeight + (distanceY - distanceX) * Constants.HorizontalTileWeight;
	}
	private List<PathNode> GetNeighbours(PathNode node)
	{
		var neighbours = new List<PathNode>();
		for (var x = -1; x <= 1; x++)
		{
			for (var y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				var neighbourX = node.Tile.Coordinates.X + x;
				var neighbourY = node.Tile.Coordinates.Y + y;
				if (neighbourX < 0 || neighbourX >= _map.GetLength(0) || neighbourY < 0 || neighbourY >= _map.GetLength(1))
				{
					continue;
				}

				neighbours.Add(new PathNode(_map[neighbourX, neighbourY]));
			}
		}

		return neighbours;
	}
	private LinkedList<PathNode> RetracePath(PathNode from, PathNode to)
	{
		var path = new LinkedList<PathNode>();
		path.AddFirst(to);

		var current = to.Parent;
		while (current != null)
		{
			path.AddFirst(current);
			current = current.Parent;
		}

		return path;
	}

	private void OnDrawGizmos()
	{
		if (_map != null)
		{
			foreach (var tile in _map)
			{
				if (tile.IsConfigured || tile.Type != TileType.Floor)
				{
					continue;
				}

				Gizmos.color = Color.green;
				//Gizmos.DrawCube(new Vector3(tile.Coordinates.X, 0.5f, tile.Coordinates.Y), Vector3.one * 0.25f);
			}
		}
	}
}