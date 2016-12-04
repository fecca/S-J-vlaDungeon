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
		Destroy(GameObject.Find("Floor"));
		Destroy(GameObject.Find("Walls"));
		Destroy(GameObject.Find("Roof"));

		MessageHub.Instance.Publish(new MeshDestroyedEvent(null));
	}
	private IEnumerator GenerateMesh(Tile[,] map, Action completed)
	{
		var mapWidth = map.GetLength(0) - 1;
		var mapHeight = map.GetLength(1) - 1;
		for (var x = 0; x < mapWidth; x++)
		{
			for (var y = 0; y < mapHeight; y++)
			{
				GenerateTileMesh(map[x, y]);
			}
		}
		for (var x = 0; x < mapWidth; x++)
		{
			for (var y = 0; y < mapHeight; y++)
			{
				GenerateWalls(map[x, y]);
			}
		}
		CreateRoofMesh();
		CreateFloorMesh();
		CreateWaterMesh();
		CreateWallMesh();

		completed();

		yield break;
	}

	private void GenerateTileMesh(Tile tile)
	{
		switch (tile.Type)
		{
			case TileType.Floor:
				CreateFloorTriangle(
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize));
				CreateFloorTriangle(
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates,
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize));
				break;
			case TileType.Roof:
				CreateRoofTriangle(
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize));
				CreateRoofTriangle(
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates,
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize));
				break;
			case TileType.Water:
				CreateWaterTriangle(
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize));
				CreateWaterTriangle(
					tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
					tile.WorldCoordinates,
					tile.WorldCoordinates + (Vector3.forward * Constants.TileSize));
				break;
			default:
				break;
		}
	}
	private void GenerateWalls(Tile tile)
	{
		switch (tile.Type)
		{
			case TileType.Floor:
				if (tile.LeftNeighbour != null && tile.LeftNeighbour.Type == TileType.Water)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates,
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize));
					CreateWallTriangle(
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize));
				}

				if (tile.TopNeighbour != null && tile.TopNeighbour.Type == TileType.Water)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.TopNeighbour.WorldCoordinates);
					CreateWallTriangle(
						tile.TopNeighbour.WorldCoordinates,
						tile.TopNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize));
				}

				if (tile.RightNeighbour != null && tile.RightNeighbour.Type == TileType.Water)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.RightNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize));
					CreateWallTriangle(
						tile.RightNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.RightNeighbour.WorldCoordinates,
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize));
				}

				if (tile.BottomNeighbour != null && tile.BottomNeighbour.Type == TileType.Water)
				{
					CreateWallTriangle(
						tile.WorldCoordinates,
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize));
					CreateWallTriangle(
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize),
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates);
				}
				break;
			case TileType.Roof:
				if (tile.LeftNeighbour != null && tile.LeftNeighbour.Type != tile.Type)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates,
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize));
					CreateWallTriangle(
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.LeftNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize));
				}

				if (tile.TopNeighbour != null && tile.TopNeighbour.Type != tile.Type)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.TopNeighbour.WorldCoordinates);
					CreateWallTriangle(
						tile.TopNeighbour.WorldCoordinates,
						tile.TopNeighbour.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize));
				}

				if (tile.RightNeighbour != null && tile.RightNeighbour.Type != tile.Type)
				{
					CreateWallTriangle(
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize) + (Vector3.forward * Constants.TileSize),
						tile.RightNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize));
					CreateWallTriangle(
						tile.RightNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.RightNeighbour.WorldCoordinates,
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize));
				}

				if (tile.BottomNeighbour != null && tile.BottomNeighbour.Type != tile.Type)
				{
					CreateWallTriangle(
						tile.WorldCoordinates,
						tile.WorldCoordinates + (Vector3.right * Constants.TileSize),
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize));
					CreateWallTriangle(
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize) + (Vector3.right * Constants.TileSize),
						tile.BottomNeighbour.WorldCoordinates + (Vector3.forward * Constants.TileSize),
						tile.WorldCoordinates);
				}
				break;
			case TileType.Water:
				break;
			default:
				break;
		}
	}

	private void CreateRoofTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		_roofVertices.Add(a);
		_roofVertices.Add(b);
		_roofVertices.Add(c);

		_roofTriangles.Add(_roofVertices.Count - 3);
		_roofTriangles.Add(_roofVertices.Count - 2);
		_roofTriangles.Add(_roofVertices.Count - 1);
	}
	private void CreateRoofMesh()
	{
		var roofGameObject = new GameObject("Roof");
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

	private void CreateFloorTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		_floorVertices.Add(a);
		_floorVertices.Add(b);
		_floorVertices.Add(c);

		_floorTriangles.Add(_floorVertices.Count - 3);
		_floorTriangles.Add(_floorVertices.Count - 2);
		_floorTriangles.Add(_floorVertices.Count - 1);
	}
	private void CreateFloorMesh()
	{
		var floorGameObject = new GameObject("Floor");
		//floorGameObject.transform.position -= Vector3.up * WallHeight * 1.0f;
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

	private void CreateWallTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		_wallVertices.Add(a);
		_wallVertices.Add(b);
		_wallVertices.Add(c);

		_wallTriangles.Add(_wallVertices.Count - 3);
		_wallTriangles.Add(_wallVertices.Count - 2);
		_wallTriangles.Add(_wallVertices.Count - 1);
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

	private void CreateWaterTriangle(Vector3 a, Vector3 b, Vector3 c)
	{
		_waterVertices.Add(a);
		_waterVertices.Add(b);
		_waterVertices.Add(c);

		_waterTriangles.Add(_waterVertices.Count - 3);
		_waterTriangles.Add(_waterVertices.Count - 2);
		_waterTriangles.Add(_waterVertices.Count - 1);
	}
	private void CreateWaterMesh()
	{
		var waterGameObject = new GameObject("Water");
		//waterGameObject.transform.position -= Vector3.up * WallHeight * 2.0f;
		waterGameObject.layer = LayerMask.NameToLayer("Water");

		var meshRenderer = waterGameObject.GetOrAddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = Resources.Load<Material>("WaterMaterial");

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