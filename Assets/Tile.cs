using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public Coordinates Coordinates;
	public TileType Type;
	public ConfigurationSquare ConfigurationSquare;
	public List<Vector3> CoreVertices = new List<Vector3>(4);

	public bool IsConfigured
	{
		get { return ConfigurationSquare != null && ConfigurationSquare.Configuration > 0 && ConfigurationSquare.Configuration < 15; }
	}

	public Tile(int x, int y, TileType type)
	{
		Coordinates = new Coordinates(x, y);
		Type = type;
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
			", Coordinates: " + Coordinates +
			", CoreVertices: " + CoreVertices.Count +
			", IsConfigured: " + IsConfigured +
			", Configuration: " + ConfigurationSquare;
	}
}