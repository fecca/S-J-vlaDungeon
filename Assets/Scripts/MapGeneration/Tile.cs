using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public TileType Type;
	public Coordinates GridCoordinates;
	public Vector3 WorldCoordinates;
	public ConfigurationSquare ConfigurationSquare;
	public List<Vector3> CoreVertices = new List<Vector3>(4);
	public Tile LeftNeighbour;
	public Tile TopNeighbour;
	public Tile RightNeighbour;
	public Tile BottomNeighbour;

	public Tile(int x, int y, TileType type)
	{
		GridCoordinates = new Coordinates(x, y);
		WorldCoordinates = new Vector3(x * Constants.TileSize, 0, y * Constants.TileSize);
		Type = type;
	}

	public override string ToString()
	{
		return "Type: " + Type +
			", Grid: " + GridCoordinates +
			", World: " + WorldCoordinates +
			", CoreVertices: " + CoreVertices.Count +
			", IsConfigured: " + (ConfigurationSquare != null) +
			", Configuration: " + (ConfigurationSquare != null ? ConfigurationSquare.Configuration.ToString() : "N/A");
	}

	public void SetConfiguration(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
	{
		ConfigurationSquare = new ConfigurationSquare(topLeft, topRight, bottomRight, bottomLeft);
		if (ConfigurationSquare != null && ConfigurationSquare.Configuration > 0 && ConfigurationSquare.Configuration < 15)
		{
			Type = TileType.Floor;
		}
	}
	public void AddWallVertices(params Vector3[] vertices)
	{
		for (var i = 0; i < vertices.Length; i++)
		{
			CoreVertices.Add(vertices[i]);
		}
	}
}