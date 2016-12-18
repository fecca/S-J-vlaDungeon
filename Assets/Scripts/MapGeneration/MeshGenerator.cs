using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	private readonly List<Vector3> _groundVertices = new List<Vector3>(16184);
	private readonly List<int> _groundTriangles = new List<int>(32768);
	private readonly List<Vector3> _wallVertices = new List<Vector3>(16184);
	private readonly List<int> _wallTriangles = new List<int>(32768);
	private readonly List<Vector3> _waterVertices = new List<Vector3>(16184);
	private readonly List<int> _waterTriangles = new List<int>(32768);

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
		_groundVertices.Clear();
		_groundTriangles.Clear();
		_wallVertices.Clear();
		_wallTriangles.Clear();
		_waterVertices.Clear();
		_waterTriangles.Clear();

		Destroy(GameObject.Find("Ground"));
		Destroy(GameObject.Find("Wall"));
		Destroy(GameObject.Find("Water"));

		MessageHub.Instance.Publish(new MeshDestroyedEvent(null));
	}

	private IEnumerator GenerateMesh(Tile[,] map, Action completed)
	{
		var mapWidth = map.GetLength(0);
		var mapHeight = map.GetLength(1);
		for (var x = 0; x < mapWidth; x++)
		{
			for (var y = 0; y < mapHeight; y++)
			{
				GenerateTileMesh(map[x, y]);
			}
		}
		CreateWallMesh();
		CreateGroundMesh();
		CreateWaterMesh();

		completed();

		yield break;
	}
	private void GenerateTileMesh(Tile tile)
	{
		var bottomLeft = tile.WorldCoordinates;
		var topLeft = tile.WorldCoordinates + (Vector3.forward * Constants.TileSize);
		var topRight = tile.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize);
		var bottomRight = tile.WorldCoordinates + (Vector3.right * Constants.TileSize);

		var topTile = tile.TileNeighbours.Top;
		var topRightTile = tile.TileNeighbours.TopRight;
		var rightTile = tile.TileNeighbours.Right;

		if (topTile != null)
		{
			topLeft = topTile.WorldCoordinates;
		}
		if (topRightTile != null)
		{
			topRight = topRightTile.WorldCoordinates;
		}
		if (rightTile != null)
		{
			bottomRight = rightTile.WorldCoordinates;
		}

		var topTriangleType = tile.Type;
		var bottomTriangleType = tile.Type;
		if (tile.Type == TileType.Wall || topTile.Type == TileType.Wall || topRightTile.Type == TileType.Wall)
		{
			topTriangleType = TileType.Wall;
		}
		else if (tile.Type == TileType.Ground || topTile.Type == TileType.Ground || topRightTile.Type == TileType.Ground)
		{
			topTriangleType = TileType.Ground;
		}
		else
		{
			topTriangleType = TileType.Water;
		}

		if (tile.Type == TileType.Wall || topRightTile.Type == TileType.Wall || rightTile.Type == TileType.Wall)
		{
			bottomTriangleType = TileType.Wall;
		}
		else if (tile.Type == TileType.Ground || topRightTile.Type == TileType.Ground || rightTile.Type == TileType.Ground)
		{
			bottomTriangleType = TileType.Ground;
		}
		else
		{
			bottomTriangleType = TileType.Water;
		}

		switch (tile.Type)
		{
			case TileType.Wall:
				break;

			case TileType.Ground:
				if (topTile.Type == TileType.Wall || topRightTile.Type == TileType.Wall || rightTile.Type == TileType.Wall)
				{
					tile.Type = TileType.Wall;
				}
				break;

			case TileType.Water:
				if (topTile.Type != tile.Type)
				{
					tile.Type = topTile.Type;
				}
				else if (topRightTile.Type != tile.Type)
				{
					tile.Type = topRightTile.Type;
				}
				else if (rightTile.Type != tile.Type)
				{
					tile.Type = rightTile.Type;
				}
				break;

			default:
				break;
		}

		CreateTriangle(topTriangleType, bottomLeft, topLeft, topRight);
		CreateTriangle(bottomTriangleType, topRight, bottomRight, bottomLeft);
	}
	private void CreateTriangle(TileType type, Vector3 a, Vector3 b, Vector3 c)
	{
		switch (type)
		{
			case TileType.Ground:
				_groundVertices.Add(a);
				_groundVertices.Add(b);
				_groundVertices.Add(c);

				_groundTriangles.Add(_groundVertices.Count - 3);
				_groundTriangles.Add(_groundVertices.Count - 2);
				_groundTriangles.Add(_groundVertices.Count - 1);
				break;
			case TileType.Wall:
				_wallVertices.Add(a);
				_wallVertices.Add(b);
				_wallVertices.Add(c);

				_wallTriangles.Add(_wallVertices.Count - 3);
				_wallTriangles.Add(_wallVertices.Count - 2);
				_wallTriangles.Add(_wallVertices.Count - 1);
				break;
			case TileType.Water:
				_waterVertices.Add(a);
				_waterVertices.Add(b);
				_waterVertices.Add(c);

				_waterTriangles.Add(_waterVertices.Count - 3);
				_waterTriangles.Add(_waterVertices.Count - 2);
				_waterTriangles.Add(_waterVertices.Count - 1);
				break;

			default:
				break;
		}
	}
	private void CreateWallMesh()
	{
		var wallGameObject = new GameObject("Wall");
		wallGameObject.layer = LayerMask.NameToLayer("Wall");
		wallGameObject.tag = "Wall";

		var meshRenderer = wallGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/WallMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_wallVertices);
		mesh.SetTriangles(_wallTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = wallGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = wallGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void CreateGroundMesh()
	{
		var groundGameObject = new GameObject("Ground");
		groundGameObject.layer = LayerMask.NameToLayer("Ground");
		groundGameObject.tag = "Ground";

		var meshRenderer = groundGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/GroundMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_groundVertices);
		mesh.SetTriangles(_groundTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = groundGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = groundGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void CreateWaterMesh()
	{
		var waterGameObject = new GameObject("Water");
		waterGameObject.layer = LayerMask.NameToLayer("Water");
		waterGameObject.tag = "Water";

		var meshRenderer = waterGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/WaterMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_waterVertices);
		mesh.SetTriangles(_waterTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = waterGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = waterGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
}