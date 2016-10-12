using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public Coordinates Coordinates;
	public TileType Type;
	public ConfigurationSquare ConfigurationSquare;
	public List<Vector3> CoreVertices = new List<Vector3>(4);
	public bool IsWallTile;

	public Tile(int x, int y, TileType type)
	{
		Coordinates = new Coordinates(x, y);
		Type = type;
	}

	public void AddWallVertices(params Vector3[] vertices)
	{
		if (!IsWallTile && vertices.Length > 0)
		{
			IsWallTile = true;
		}

		for (var i = 0; i < vertices.Length; i++)
		{
			CoreVertices.Add(vertices[i]);
		}
	}
}