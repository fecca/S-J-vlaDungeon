using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public TileType Type;
	public Coordinates GridCoordinates;
	public Coordinates WorldCoordinates;
	public ConfigurationSquare ConfigurationSquare;
	public List<Vector3> CoreVertices = new List<Vector3>(4);

	public bool IsWallTile
	{
		get
		{
			return ConfigurationSquare != null && ConfigurationSquare.Configuration > 0 && ConfigurationSquare.Configuration < 15;
		}
	}
	public bool IsWalkable
	{
		get { return Type == TileType.Floor && ConfigurationSquare != null && ConfigurationSquare.Configuration == 15; }
	}

	public Tile(int x, int y, TileType type)
	{
		GridCoordinates = new Coordinates(x, y);
		WorldCoordinates = new Coordinates(x * Constants.TileSize, y * Constants.TileSize);
		Type = type;
	}
	public void SetConfiguration(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
	{
		ConfigurationSquare = new ConfigurationSquare(topLeft, topRight, bottomRight, bottomLeft);
		if (ConfigurationSquare.Configuration > 0)
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
	public override string ToString()
	{
		return "Type: " + Type +
			", Coordinates: " + WorldCoordinates +
			", CoreVertices: " + CoreVertices.Count +
			", IsConfigured: " + IsWalkable +
			", Configuration: " + ConfigurationSquare;
	}
}