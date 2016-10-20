using System.Collections.Generic;

public class PathfindingNode : IHeapItem<PathfindingNode>
{
	public PathfindingNode Parent;
	public Coordinates GridCoordinates;
	public Coordinates WorldCoordinates;
	public List<PathfindingNode> Neighbours;
	public bool Walkable;

	public float GCost { get; set; }
	public float HCost { get; set; }
	public float FCost { get; set; }
	public int HeapIndex { get; set; }

	public PathfindingNode(float x, float y, bool walkable, int tileSize)
	{
		GridCoordinates = new Coordinates(x, y);
		WorldCoordinates = new Coordinates((x / 2f + 0.25f) * tileSize, (y / 2f + 0.25f) * tileSize);
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
		return "Grid: [" + GridCoordinates.X + ";" + GridCoordinates.Y + "], World: [" + WorldCoordinates.X + ";" + WorldCoordinates.Y + "], Walkable: " + Walkable;
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
}