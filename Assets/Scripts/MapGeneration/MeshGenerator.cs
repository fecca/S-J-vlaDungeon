using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	private readonly List<Vector3> _floorVertices = new List<Vector3>(16184);
	private readonly List<int> _floorTriangles = new List<int>(32768);
	private readonly List<Vector3> _wallVertices = new List<Vector3>(16184);
	private readonly List<int> _wallTriangles = new List<int>(32768);
	private readonly List<Vector3> _roofVertices = new List<Vector3>(16184);
	private readonly List<int> _roofTriangles = new List<int>(32768);

	[SerializeField]
	private int WallHeight = 2;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<MapCreatedEvent>(OnMapCreatedEvent);
		MessageHub.Instance.Subscribe<PathNodesDestroyedEvent>(OnPathNodesDestroyedEvent);
	}

	private void OnMapCreatedEvent(MapCreatedEvent mapCreatedEvent)
	{
		StartCoroutine(GenerateMesh(mapCreatedEvent.Map, () =>
		{
			MessageHub.Instance.Publish(new MeshCreatedEvent(null)
			{
				Map = mapCreatedEvent.Map
			});
		}));
	}
	private void OnPathNodesDestroyedEvent(PathNodesDestroyedEvent pathNodesDestroyedEvent)
	{
		Destroy(GameObject.Find("Floor"));
		Destroy(GameObject.Find("Walls"));
		Destroy(GameObject.Find("Roof"));

		MessageHub.Instance.Publish(new MeshDestroyedEvent(null));
	}
	private IEnumerator GenerateMesh(Tile[,] map, Action completed)
	{
		GenerateFloorMesh(map);
		GenerateWallMesh(map);
		GenerateRoofMesh(map);

		completed();

		yield break;
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
				BuildFloorTriangleFromPoints(square.BottomLeft, square.CenterLeft, square.CenterBottom);
				break;

			case 2:
				BuildFloorTriangleFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;

			case 4:
				BuildFloorTriangleFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;

			case 8:
				BuildFloorTriangleFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 points
			case 3:
				BuildFloorTriangleFromPoints(square.BottomLeft, square.CenterLeft, square.CenterRight, square.BottomRight);
				break;

			case 6:
				BuildFloorTriangleFromPoints(square.BottomRight, square.CenterBottom, square.CenterTop, square.TopRight);
				break;

			case 12:
				BuildFloorTriangleFromPoints(square.TopRight, square.CenterRight, square.CenterLeft, square.TopLeft);
				break;

			case 9:
				BuildFloorTriangleFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;

			case 5:
				BuildFloorTriangleFromPoints(square.BottomLeft, square.CenterBottom, square.CenterRight, square.TopRight, square.CenterTop, square.CenterLeft);
				break;

			case 10:
				BuildFloorTriangleFromPoints(square.BottomRight, square.CenterRight, square.CenterTop, square.TopLeft, square.CenterLeft, square.CenterBottom);
				break;

			// 3 points
			case 11:
				BuildFloorTriangleFromPoints(square.BottomLeft, square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight);
				break;

			case 7:
				BuildFloorTriangleFromPoints(square.BottomRight, square.BottomLeft, square.CenterLeft, square.CenterTop, square.TopRight);
				break;

			case 14:
				BuildFloorTriangleFromPoints(square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft, square.TopLeft);
				break;

			case 13:
				BuildFloorTriangleFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
				break;

			// 4 points
			case 15:
				BuildFloorTriangleFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				break;
		}
	}
	private void CreateFloorMesh()
	{
		var floorGameObject = new GameObject("Floor");
		floorGameObject.layer = LayerMask.NameToLayer("Ground");

		var meshRenderer = floorGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("FloorMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_floorVertices);
		mesh.SetTriangles(_floorTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = floorGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = floorGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void BuildFloorTriangleFromPoints(params Node[] points)
	{
		if (points.Length == 3)
		{
			CreateFloorTriangle(points[0], points[1], points[2]);
		}
		if (points.Length == 4)
		{
			CreateFloorTriangle(points[0], points[1], points[2]);
			CreateFloorTriangle(points[0], points[2], points[3]);
		}
		if (points.Length == 5)
		{
			CreateFloorTriangle(points[0], points[1], points[2]);
			CreateFloorTriangle(points[0], points[2], points[3]);
			CreateFloorTriangle(points[0], points[3], points[4]);
		}
		if (points.Length == 6)
		{
			CreateFloorTriangle(points[0], points[1], points[2]);
			CreateFloorTriangle(points[0], points[2], points[3]);
			CreateFloorTriangle(points[0], points[3], points[4]);
			CreateFloorTriangle(points[0], points[4], points[5]);
		}
	}
	private void CreateFloorTriangle(Node a, Node b, Node c)
	{
		_floorVertices.Add(a.Position);
		_floorVertices.Add(b.Position);
		_floorVertices.Add(c.Position);

		_floorTriangles.Add(_floorVertices.Count - 3);
		_floorTriangles.Add(_floorVertices.Count - 2);
		_floorTriangles.Add(_floorVertices.Count - 1);
	}

	private void GenerateWallMesh(Tile[,] map)
	{
		_wallVertices.Clear();
		_wallTriangles.Clear();
		for (var x = 0; x < map.GetLength(0); x++)
		{
			for (var y = 0; y < map.GetLength(1); y++)
			{
				if (!map[x, y].IsEdgeTile)
				{
					continue;
				}
				TriangulateWall(map[x, y]);
			}
		}

		CreateWallMesh();
	}
	private void TriangulateWall(Tile tile)
	{
		var square = tile.ConfigurationSquare;
		if (square == null)
		{
			return;
		}

		switch (square.Configuration)
		{
			// 0 points
			case 0:
				break;

			// 1 points
			case 1:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterLeft.Position);
				break;

			case 2:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterBottom.Position);
				break;

			case 4:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterRight.Position);
				break;

			case 8:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterTop.Position);
				break;

			// 2 points
			case 3:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterLeft.Position);
				break;

			case 6:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterBottom.Position);
				break;

			case 12:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterRight.Position);
				break;

			case 9:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterTop.Position);
				break;

			case 5:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterBottom.Position);
				break;

			case 10:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterRight.Position);
				break;

			// 3 points
			case 11:
				tile.AddWallVertices(square.CenterRight.Position, square.CenterTop.Position);
				break;

			case 7:
				tile.AddWallVertices(square.CenterTop.Position, square.CenterLeft.Position);
				break;

			case 14:
				tile.AddWallVertices(square.CenterLeft.Position, square.CenterBottom.Position);
				break;

			case 13:
				tile.AddWallVertices(square.CenterBottom.Position, square.CenterRight.Position);
				break;

			// 4 points
			case 15:
				break;
		}

		if (tile.IsEdgeTile)
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
	private void CreateWallMesh()
	{
		var wallGameObject = new GameObject("Walls");
		wallGameObject.layer = LayerMask.NameToLayer("Wall");

		var meshRenderer = wallGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("WallMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_wallVertices);
		mesh.SetTriangles(_wallTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = wallGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = wallGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}

	private void GenerateRoofMesh(Tile[,] map)
	{
		_roofVertices.Clear();
		_roofTriangles.Clear();
		for (var x = 0; x < map.GetLength(0) - 1; x++)
		{
			for (var y = 0; y < map.GetLength(1) - 1; y++)
			{
				TriangulateRoof(map[x, y]);
			}
		}

		CreateRoofMesh();
	}
	private void TriangulateRoof(Tile tile)
	{
		var square = tile.ConfigurationSquare;
		var configuration = 15 - square.Configuration;
		switch (configuration)
		{
			// 0 points
			case 0:
				break;

			// 1 points
			case 1:
				BuildRoofTriangleFromPoints(square.BottomLeft, square.CenterLeft, square.CenterBottom);
				break;

			case 2:
				BuildRoofTriangleFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;

			case 4:
				BuildRoofTriangleFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;

			case 8:
				BuildRoofTriangleFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 points
			case 3:
				BuildRoofTriangleFromPoints(square.BottomLeft, square.CenterLeft, square.CenterRight, square.BottomRight);
				break;

			case 6:
				BuildRoofTriangleFromPoints(square.BottomRight, square.CenterBottom, square.CenterTop, square.TopRight);
				break;

			case 12:
				BuildRoofTriangleFromPoints(square.TopRight, square.CenterRight, square.CenterLeft, square.TopLeft);
				break;

			case 9:
				BuildRoofTriangleFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;

			case 5:
				BuildRoofTriangleFromPoints(square.BottomLeft, square.CenterBottom, square.CenterRight, square.TopRight, square.CenterTop, square.CenterLeft);
				break;

			case 10:
				BuildRoofTriangleFromPoints(square.BottomRight, square.CenterRight, square.CenterTop, square.TopLeft, square.CenterLeft, square.CenterBottom);
				break;

			// 3 points
			case 11:
				BuildRoofTriangleFromPoints(square.BottomLeft, square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight);
				break;

			case 7:
				BuildRoofTriangleFromPoints(square.BottomRight, square.BottomLeft, square.CenterLeft, square.CenterTop, square.TopRight);
				break;

			case 14:
				BuildRoofTriangleFromPoints(square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft, square.TopLeft);
				break;

			case 13:
				BuildRoofTriangleFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
				break;

			// 4 points
			case 15:
				BuildRoofTriangleFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				break;
		}
	}
	private void CreateRoofMesh()
	{
		var roofGameObject = new GameObject("Roof");
		roofGameObject.transform.position += Vector3.up * WallHeight;
		roofGameObject.layer = LayerMask.NameToLayer("Roof");

		var meshRenderer = roofGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("RoofMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_roofVertices);
		mesh.SetTriangles(_roofTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = roofGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = roofGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void BuildRoofTriangleFromPoints(params Node[] points)
	{
		if (points.Length == 3)
		{
			CreateRoofTriangle(points[0], points[1], points[2]);
		}
		if (points.Length == 4)
		{
			CreateRoofTriangle(points[0], points[1], points[2]);
			CreateRoofTriangle(points[0], points[2], points[3]);
		}
		if (points.Length == 5)
		{
			CreateRoofTriangle(points[0], points[1], points[2]);
			CreateRoofTriangle(points[0], points[2], points[3]);
			CreateRoofTriangle(points[0], points[3], points[4]);
		}
		if (points.Length == 6)
		{
			CreateRoofTriangle(points[0], points[1], points[2]);
			CreateRoofTriangle(points[0], points[2], points[3]);
			CreateRoofTriangle(points[0], points[3], points[4]);
			CreateRoofTriangle(points[0], points[4], points[5]);
		}
	}
	private void CreateRoofTriangle(Node a, Node b, Node c)
	{
		_roofVertices.Add(a.Position);
		_roofVertices.Add(b.Position);
		_roofVertices.Add(c.Position);

		_roofTriangles.Add(_roofVertices.Count - 3);
		_roofTriangles.Add(_roofVertices.Count - 2);
		_roofTriangles.Add(_roofVertices.Count - 1);
	}
}