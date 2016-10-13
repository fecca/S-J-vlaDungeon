using System;

public class PathNode
{
	public Tile Tile;
	public PathNode Parent;

	public bool IsWalkable
	{
		get { return Tile.Type == TileType.Floor && !Tile.IsConfigured; }
	}
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost
	{
		get
		{
			return GCost + HCost;
		}
	}

	public PathNode(Tile tile)
	{
		Tile = tile;
	}

	public override bool Equals(object other)
	{
		return Tile == ((PathNode)other).Tile;
	}
}