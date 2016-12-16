using UnityEngine;

public class Tile
{
	public TileType Type;
	public Point GridCoordinates;
	public Vector3 WorldCoordinates;
	public TileNeighbours TileNeighbours;

	public Tile(int x, int y, TileType type)
	{
		GridCoordinates = new Point(x, y);
		WorldCoordinates = new Vector3(x * Constants.TileSize, 0, y * Constants.TileSize);
		Type = type;
	}
}