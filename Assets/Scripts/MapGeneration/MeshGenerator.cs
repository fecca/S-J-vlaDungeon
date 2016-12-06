using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	private readonly List<Vector3> _floorVertices = new List<Vector3>(16184);
	private readonly List<int> _floorTriangles = new List<int>(32768);
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
		Destroy(GameObject.Find("Roof"));
		Destroy(GameObject.Find("Water"));

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
		CreateRoofMesh();
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

		switch (tile.Type)
		{
			case TileType.Roof:
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

				CreateTriangle(tile.Type, bottomLeft, topLeft, topRight);
				CreateTriangle(tile.Type, topRight, bottomRight, bottomLeft);

				break;

			case TileType.Floor:
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

				if (topTile.Type == TileType.Roof || topRightTile.Type == TileType.Roof || rightTile.Type == TileType.Roof)
				{
					tile.Type = TileType.Roof;
				}

				var topFloorTriangleType = tile.Type;
				if (tile.WorldCoordinates.y < 1.0f && tile.WorldCoordinates.y > -1.0f)
				{
					if (topLeft.y < 1.0f && topLeft.y > -1.0f)
					{
						if (topRight.y < 1.0f && topRight.y > -1.0f)
						{
							topFloorTriangleType = TileType.Floor;
						}
					}
				}

				var bottomFloorTriangleType = tile.Type;
				if (tile.WorldCoordinates.y < 1.0f && tile.WorldCoordinates.y > -1.0f)
				{
					if (bottomRight.y < 1.0f && bottomRight.y > -1.0f)
					{
						if (topRight.y < 1.0f && topRight.y > -1.0f)
						{
							bottomFloorTriangleType = TileType.Floor;
						}
					}
				}

				CreateTriangle(topFloorTriangleType, bottomLeft, topLeft, topRight);
				CreateTriangle(bottomFloorTriangleType, topRight, bottomRight, bottomLeft);

				break;

			case TileType.Water:
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

				var topWaterTriangleType = tile.Type;
				if (tile.WorldCoordinates.y < -1.0f && tile.WorldCoordinates.y > -3.0f)
				{
					if (topLeft.y < -1.0f && topLeft.y > -3.0f)
					{
						if (topRight.y < -1.0f && topRight.y > -3.0f)
						{
							topWaterTriangleType = TileType.Water;
						}
					}
				}

				var bottomWaterTriangleType = tile.Type;
				if (tile.WorldCoordinates.y < -1.0f && tile.WorldCoordinates.y > -3.0f)
				{
					if (bottomRight.y < -1.0f && bottomRight.y > -3.0f)
					{
						if (topRight.y < -1.0f && topRight.y > -3.0f)
						{
							bottomWaterTriangleType = TileType.Water;
						}
					}
				}

				CreateTriangle(topWaterTriangleType, bottomLeft, topLeft, topRight);
				CreateTriangle(bottomWaterTriangleType, topRight, bottomRight, bottomLeft);

				break;

			default:
				break;
		}
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
			case TileType.Roof:
				_roofVertices.Add(a);
				_roofVertices.Add(b);
				_roofVertices.Add(c);

				_roofTriangles.Add(_roofVertices.Count - 3);
				_roofTriangles.Add(_roofVertices.Count - 2);
				_roofTriangles.Add(_roofVertices.Count - 1);
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
	private void CreateWaterMesh()
	{
		var waterGameObject = new GameObject("Water");
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