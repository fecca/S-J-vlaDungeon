using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	private readonly List<Vector3> _floorVertices = new List<Vector3>(16184);
	private readonly List<int> _floorTriangles = new List<int>(32768);
	private readonly List<Vector3> _WallVertices = new List<Vector3>(16184);
	private readonly List<int> _WallTriangles = new List<int>(32768);
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
		_floorVertices.Clear();
		_floorTriangles.Clear();
		_WallVertices.Clear();
		_WallTriangles.Clear();
		_waterVertices.Clear();
		_waterTriangles.Clear();

		Destroy(GameObject.Find("Floor"));
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
		CreateFloorMesh();
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
		else if (tile.Type == TileType.Floor || topTile.Type == TileType.Floor || topRightTile.Type == TileType.Floor)
		{
			topTriangleType = TileType.Floor;
		}
		else
		{
			topTriangleType = TileType.Water;
		}

		if (tile.Type == TileType.Wall || topRightTile.Type == TileType.Wall || rightTile.Type == TileType.Wall)
		{
			bottomTriangleType = TileType.Wall;
		}
		else if (tile.Type == TileType.Floor || topRightTile.Type == TileType.Floor || rightTile.Type == TileType.Floor)
		{
			bottomTriangleType = TileType.Floor;
		}
		else
		{
			bottomTriangleType = TileType.Water;
		}

		switch (tile.Type)
		{
			case TileType.Wall:
				break;

			case TileType.Floor:
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
			case TileType.Floor:
				_floorVertices.Add(a);
				_floorVertices.Add(b);
				_floorVertices.Add(c);

				_floorTriangles.Add(_floorVertices.Count - 3);
				_floorTriangles.Add(_floorVertices.Count - 2);
				_floorTriangles.Add(_floorVertices.Count - 1);
				break;
			case TileType.Wall:
				_WallVertices.Add(a);
				_WallVertices.Add(b);
				_WallVertices.Add(c);

				_WallTriangles.Add(_WallVertices.Count - 3);
				_WallTriangles.Add(_WallVertices.Count - 2);
				_WallTriangles.Add(_WallVertices.Count - 1);
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
		var WallGameObject = new GameObject("Wall");
		WallGameObject.layer = LayerMask.NameToLayer("Wall");

		var meshRenderer = WallGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/WallMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_WallVertices);
		mesh.SetTriangles(_WallTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = WallGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = WallGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void CreateFloorMesh()
	{
		var floorGameObject = new GameObject("Floor");
		floorGameObject.layer = LayerMask.NameToLayer("Ground");

		var meshRenderer = floorGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/FloorMaterial");

		var mesh = new Mesh();
		mesh.SetVertices(_floorVertices);
		mesh.SetTriangles(_floorTriangles, 0);
		mesh.RecalculateNormals();

		var meshFilter = floorGameObject.GetOrAddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		var meshCollider = floorGameObject.GetOrAddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	private void CreateWaterMesh()
	{
		var waterGameObject = new GameObject("Water");
		waterGameObject.layer = LayerMask.NameToLayer("Water");

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