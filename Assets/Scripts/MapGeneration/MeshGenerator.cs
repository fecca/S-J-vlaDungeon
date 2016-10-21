using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	[SerializeField]
	private GameObject Walls = null;
	[SerializeField]
	private GameObject Floor = null;
	[SerializeField]
	private int WallHeight = 2;
	[SerializeField]
	private int WallSegments = 1;
	[SerializeField]
	private bool RandomizeEdgeVertexPositions = false;

	private readonly List<Vector3> _floorVertices = new List<Vector3>(16184);
	private readonly List<int> _floorTriangles = new List<int>(32768);

	private readonly List<Vector3> _wallVertices = new List<Vector3>(16184);
	private readonly List<int> _wallTriangles = new List<int>(32768);

	public void GenerateMeshes(Tile[,] map)
	{
		GenerateFloorMesh(map);
		GenerateWallMesh(map);
	}

	private void GenerateFloorMesh(Tile[,] map)
	{
		_floorVertices.Clear();
		_floorTriangles.Clear();
		for (var x = 0; x < map.GetLength(0) - 1; x++)
		{
			for (var y = 0; y < map.GetLength(1) - 1; y++)
			{
				TriangulateFloor(map[x, y]);
			}
		}

		CreateFloorMesh();
	}

	private void GenerateWallMesh(Tile[,] map)
	{
		_wallVertices.Clear();
		_wallTriangles.Clear();
		for (var x = 0; x < map.GetLength(0); x++)
		{
			for (var y = 0; y < map.GetLength(1); y++)
			{
				TriangulateWall(map[x, y]);
			}
		}

		CreateWallMesh();
	}

	private void TriangulateFloor(Tile tile)
	{
		var square = tile.ConfigurationSquare;
		switch (square.Configuration)
		{
			// 0 points
			case 0:
				break;

			// 1 points
			case 1:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterLeft, square.CenterBottom);
				break;
			case 2:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;
			case 4:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;
			case 8:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 points
			case 3:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterLeft, square.CenterRight, square.BottomRight);
				break;
			case 6:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterTop, square.TopRight);
				break;
			case 12:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopRight, square.CenterRight, square.CenterLeft, square.TopLeft);
				break;
			case 9:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;
			case 5:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterBottom, square.CenterRight, square.TopRight, square.CenterTop, square.CenterLeft);
				break;
			case 10:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterRight, square.CenterTop, square.TopLeft, square.CenterLeft, square.CenterBottom);
				break;

			// 3 points
			case 11:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.BottomLeft, square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight);
				break;
			case 7:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomRight, square.BottomLeft, square.CenterLeft, square.CenterTop, square.TopRight);
				break;
			case 14:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft, square.TopLeft);
				break;
			case 13:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
				break;

			// 4 points
			case 15:
				BuildmeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				break;
		}
	}

	private void BuildmeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if (points.Length == 3)
		{
			CreateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length == 4)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length == 5)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
			CreateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length == 6)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
			CreateTriangle(points[0], points[3], points[4]);
			CreateTriangle(points[0], points[4], points[5]);
		}
	}

	private void AssignVertices(Node[] points)
	{
		for (var i = 0; i < points.Length; i++)
		{
			var point = points[i];
			if (point.VertexIndex == -1)
			{
				point.VertexIndex = _floorVertices.Count;

				if (RandomizeEdgeVertexPositions)
				{
					point.Position += Vector3.right * Random.Range(-0.1f, 0.1f);
					point.Position += Vector3.forward * Random.Range(-0.1f, 0.1f);
				}

				_floorVertices.Add(point.Position);
			}
		}
	}

	private void CreateTriangle(Node a, Node b, Node c)
	{
		_floorTriangles.Add(a.VertexIndex);
		_floorTriangles.Add(b.VertexIndex);
		_floorTriangles.Add(c.VertexIndex);
	}

	private void TriangulateWall(Tile tile)
	{
		if (tile.IsWallTile)
		{
			_wallVertices.Add(tile.CoreVertices[0]);
			_wallVertices.Add(tile.CoreVertices[1]);
			_wallVertices.Add(tile.CoreVertices[1] + Vector3.up * WallHeight);

			_wallTriangles.Add(_wallVertices.Count - 3);
			_wallTriangles.Add(_wallVertices.Count - 2);
			_wallTriangles.Add(_wallVertices.Count - 1);

			_wallVertices.Add(tile.CoreVertices[1] + Vector3.up * WallHeight);
			_wallVertices.Add(tile.CoreVertices[0] + Vector3.up * WallHeight);
			_wallVertices.Add(tile.CoreVertices[0]);

			_wallTriangles.Add(_wallVertices.Count - 3);
			_wallTriangles.Add(_wallVertices.Count - 2);
			_wallTriangles.Add(_wallVertices.Count - 1);
		}
	}

	private void CreateFloorMesh()
	{
		var mesh = new Mesh();
		mesh.SetVertices(_floorVertices);
		mesh.SetTriangles(_floorTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = Floor.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = Floor.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}

	private void CreateWallMesh()
	{
		var mesh = new Mesh();
		mesh.SetVertices(_wallVertices);
		mesh.SetTriangles(_wallTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = Walls.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = Walls.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
}