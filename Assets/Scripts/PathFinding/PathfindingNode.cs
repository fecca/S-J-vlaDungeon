using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode : IHeapItem<PathfindingNode>
{
	public PathfindingNode Parent;
	public Coordinates GridCoordinates;
	public Vector3 WorldCoordinates;
	public List<PathfindingNode> Neighbours;
	public bool Walkable;

	public bool Occupied { get; private set; }
	public float GCost { get; set; }
	public float HCost { get; set; }
	public float FCost { get; set; }
	public int HeapIndex { get; set; }

	public PathfindingNode(int x, int y, bool walkable, int tileSize)
	{
		GridCoordinates = new Coordinates(x, y);
		WorldCoordinates = FindWorldCoordinates(x, y, tileSize);
		Neighbours = new List<PathfindingNode>();
		Walkable = walkable;
	}
	public PathfindingNode(PathfindingNode copy)
	{
		GridCoordinates = copy.GridCoordinates;
		WorldCoordinates = copy.WorldCoordinates;
		Neighbours = copy.Neighbours;
		Walkable = copy.Walkable;
	}

	public override bool Equals(object other)
	{
		var otherNode = (PathfindingNode)other;

		return GridCoordinates.X == otherNode.GridCoordinates.X && GridCoordinates.Y == otherNode.GridCoordinates.Y;
	}
	public override int GetHashCode()
	{
		return (GridCoordinates.X + ";" + GridCoordinates.Y).GetHashCode();
	}
	public override string ToString()
	{
		return string.Format("Grid: {0}, " +
			"World: {1}, " +
			"Walkable: {2}, " +
			"Occupied: {3}, ",
			GridCoordinates, WorldCoordinates, Walkable, Occupied);
	}
	public int CompareTo(PathfindingNode other)
	{
		var compare = FCost.CompareTo(other.FCost);
		if (compare == 0)
		{
			compare = HCost.CompareTo(other.HCost);
		}

		return -compare;
	}
	public void SetOccupied(bool occupied)
	{
		//for (var i = 0; i < Neighbours.Count; i++)
		//{
		//	Neighbours[i].Occupied = occupied;
		//}
		Occupied = occupied;
	}

	private Vector3 FindWorldCoordinates(float x, float y, int tileSize)
	{
		var worldPosition = new Vector3((x / 2f + 0.25f) * tileSize, 0, (y / 2f + 0.25f) * tileSize);
		Ray ray = new Ray(worldPosition + Vector3.up * 2f, -Vector3.up);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 10f))
		{
			var adjustedPoint = hit.point + Vector3.up * 0.1f;
			var newPosition = worldPosition;
			newPosition.y = adjustedPoint.y;
			return newPosition;
		}
		return worldPosition;
	}
}