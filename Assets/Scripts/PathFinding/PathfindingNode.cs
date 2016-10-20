using System.Collections.Generic;

public class PathfindingNode
{
	public Coordinates GridCoordinates;
	public Coordinates WorldCoordinates;
	public PathfindingNode Parent;
	public List<PathfindingNode> Neighbours = new List<PathfindingNode>();
	public bool Walkable;

	public float GCost { get; set; }
	public float HCost { get; set; }
	public float FCost { get; set; }

	public PathfindingNode(float x, float y, bool walkable)
	{
		GridCoordinates = new Coordinates(x, y);
		WorldCoordinates = new Coordinates(x / 2f + 0.25f, y / 2f + 0.25f);
		Walkable = walkable;
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
}