using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	private Tile[,] _tiles;
	private LinkedList<PathNode> _path;

	public void RegisterMap(Tile[,] tiles)
	{
		_tiles = tiles;

		_path = GetPath(_tiles[11, 8], _tiles[10, 17]);
	}

	public LinkedList<PathNode> GetPath(Tile from, Tile to)
	{
		var fromNode = new PathNode(from);
		if (!fromNode.IsWalkable)
		{
			throw new System.Exception("From-node is invalid.");
		}
		var toNode = new PathNode(to);
		if (!toNode.IsWalkable)
		{
			throw new System.Exception("To-node is invalid.");
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
				if (neighbourX < 0 || neighbourX >= _tiles.GetLength(0) || neighbourY < 0 || neighbourY >= _tiles.GetLength(1))
				{
					continue;
				}

				neighbours.Add(new PathNode(_tiles[neighbourX, neighbourY]));
			}
		}

		return neighbours;
	}

	private void OnDrawGizmos()
	{
		if (_tiles != null)
		{
			foreach (var tile in _tiles)
			{
				if (tile.IsConfigured || tile.Type != TileType.Floor)
				{
					continue;
				}

				Gizmos.color = Color.green;
				Gizmos.DrawCube(new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y), Vector3.one * 0.25f);
			}
		}

		if (_path != null)
		{
			for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(new Vector3(iteration.Value.Tile.Coordinates.X, 0, iteration.Value.Tile.Coordinates.Y), Vector3.one * 0.25f);
			}
		}
	}
}