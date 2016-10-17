public class PathfindingNode
{
	public float X;
	public float Y;
	public bool Walkable;
	public PathfindingNode Parent;

	public float GCost { get; set; }
	public float HCost { get; set; }
	public float FCost { get; set; }

	public PathfindingNode(float x, float y, bool walkable)
	{
		X = x;
		Y = y;
		Walkable = walkable;
	}

	public override bool Equals(object other)
	{
		var otherNode = (PathfindingNode)other;
		return X == otherNode.X && Y == otherNode.Y;
	}
	public override string ToString()
	{
		return X + ";" + Y;
	}
}