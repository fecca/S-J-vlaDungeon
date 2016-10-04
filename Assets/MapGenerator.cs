using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	private string Seed;
	[SerializeField]
	private bool UseRandomSeed;
	[SerializeField]
	private int Width;
	[SerializeField]
	private int Height;
	[SerializeField]
	[Range(0, 100)]
	private int RandomFillPercent;
	[SerializeField]
	private int BorderSize;
	[SerializeField]
	int WallThresholdSize;
	[SerializeField]
	int RoomThresholdSize;

	private int[,] map;
	private int _smoothingLevels = 5;

	private void Start()
	{
		GenerateMap();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GenerateMap();
		}
	}

	private void GenerateMap()
	{
		map = new int[Width, Height];
		RandomFillMap();
		SmoothMap();
		
		ProcessMap();

		var borderedMap = GenerateBorderedMap();
		var meshGenerator = GetComponent<MeshGenerator>();
		meshGenerator.GenerateMesh(borderedMap, 1);
	}

	private int[,] GenerateBorderedMap()
	{
		int[,] borderedMap = new int[Width + BorderSize * 2, Height + BorderSize * 2];
		for (var x = 0; x < borderedMap.GetLength(0); x++)
		{
			for (var y = 0; y < borderedMap.GetLength(1); y++)
			{
				if (x >= BorderSize && x < Width + BorderSize && y >= BorderSize && y < Height + BorderSize)
				{
					borderedMap[x, y] = map[x - BorderSize, y - BorderSize];
				}
				else
				{
					borderedMap[x, y] = 1;
				}
			}
		}

		return borderedMap;

	}

	private void RandomFillMap()
	{
		if (UseRandomSeed)
		{
			Seed = Time.time.ToString();
		}

		System.Random rng = new System.Random(Seed.GetHashCode());
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
				{
					map[x, y] = 1;
				}
				else
				{
					map[x, y] = rng.Next(0, 100) < RandomFillPercent ? 1 : 0;
				}
			}
		}
	}

	private void SmoothMap()
	{
		for (var i = 0; i < _smoothingLevels; i++)
		{
			for (var x = 1; x < Width - 1; x++)
			{
				for (var y = 1; y < Height - 1; y++)
				{
					if (GetNumberOfNeighbouringWalls(x, y) > 4)
					{
						map[x, y] = 1;
					}
					else if (GetNumberOfNeighbouringWalls(x, y) < 4)
					{
						map[x, y] = 0;
					}
				}
			}
		}
	}

	private int GetNumberOfNeighbouringWalls(int xPosition, int yPosition)
	{
		var neighbouringWalls = 0;
		for (var x = xPosition - 1; x <= xPosition + 1; x++)
		{
			for (var y = yPosition - 1; y <= yPosition + 1; y++)
			{
				if (IsInMapRange(x, y))
				{
					if (x != xPosition || y != yPosition)
					{
						neighbouringWalls += map[x, y];
					}
				}
			}
		}

		return neighbouringWalls;
	}

	private void ProcessMap()
	{
		ProcessWalls();
		ProcessRooms();
	}

	private void ProcessWalls()
	{
		var wallRegions = GetRegions(1);
		for (var i = 0; i < wallRegions.Count; i++)
		{
			var wallRegion = wallRegions[i];
			if (wallRegion.Count < WallThresholdSize)
			{
				for (var j = 0; j < wallRegion.Count; j++)
				{
					map[wallRegion[j].TileX, wallRegion[j].TileY] = 0;
				}
			}
		}
	}

	private void ProcessRooms()
	{
		var survivingRooms = new List<Room>();
		var roomRegions = GetRegions(0);
		for (var i = 0; i < roomRegions.Count; i++)
		{
			var roomRegion = roomRegions[i];
			if (roomRegion.Count < RoomThresholdSize)
			{
				for (var j = 0; j < roomRegion.Count; j++)
				{
					map[roomRegion[j].TileX, roomRegion[j].TileY] = 1;
				}
			}
			else
			{
				survivingRooms.Add(new Room(roomRegion, map));
			}
		}

		ConnectClosestRooms(survivingRooms);
	}

	private List<List<Coordinates>> GetRegions(int tileType)
	{
		var regions = new List<List<Coordinates>>();
		var mapFlags = new int[Width, Height];

		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				if (mapFlags[x, y] == 0 && map[x, y] == tileType)
				{
					var newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					for (var i = 0; i < newRegion.Count; i++)
					{
						mapFlags[newRegion[i].TileX, newRegion[i].TileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	private List<Coordinates> GetRegionTiles(int startX, int startY)
	{
		var tiles = new List<Coordinates>();
		var mapFlags = new int[Width, Height];
		var tileType = map[startX, startY];
		var queue = new Queue<Coordinates>();

		queue.Enqueue(new Coordinates(startX, startY));
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0)
		{
			var tile = queue.Dequeue();
			tiles.Add(tile);

			for (var x = tile.TileX - 1; x <= tile.TileX + 1; x++)
			{
				for (var y = tile.TileY - 1; y <= tile.TileY + 1; y++)
				{
					if (IsInMapRange(x, y) && (y == tile.TileY || x == tile.TileX))
					{
						if (mapFlags[x, y] == 0 && map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coordinates(x, y));
						}
					}
				}
			}
		}

		return tiles;
	}

	private void ConnectClosestRooms(List<Room> allRooms)
	{
		var bestDistance = 0;
		var bestTileA = new Coordinates();
		var bestTileB = new Coordinates();
		var bestRoomA = new Room();
		var bestRoomB = new Room();
		var possibleConnectionFound = false;

		for (var i = 0; i < allRooms.Count; i++)
		{
			possibleConnectionFound = false;
			for (var j = 0; j < allRooms.Count; j++)
			{
				if (allRooms[i] == allRooms[j])
				{
					continue;
				}

				if (allRooms[i].IsConnected(allRooms[j]))
				{
					possibleConnectionFound = false;
					break;
				}

				for (var tileIndexA = 0; tileIndexA < allRooms[i].EdgeTiles.Count; tileIndexA++)
				{
					for (var tileIndexB = 0; tileIndexB < allRooms[j].EdgeTiles.Count; tileIndexB++)
					{
						var tileA = allRooms[i].EdgeTiles[tileIndexA];
						var tileB = allRooms[j].EdgeTiles[tileIndexB];
						var distanceBetweenRooms = (int)(Mathf.Pow(tileA.TileX - tileB.TileX, 2) + Mathf.Pow(tileA.TileY - tileB.TileY, 2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
						{
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = allRooms[i];
							bestRoomB = allRooms[j];
						}
					}
				}
			}

			if (possibleConnectionFound)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}
	}

	private void CreatePassage(Room roomA, Room roomB, Coordinates tileA, Coordinates tileB)
	{
		roomA.ConnectedRooms.Add(roomB);
		roomB.ConnectedRooms.Add(roomA);

		Debug.Log(CoordinatesToWorldPoint(tileA) + "; " + CoordinatesToWorldPoint(tileB));
		Debug.DrawLine(CoordinatesToWorldPoint(tileA), CoordinatesToWorldPoint(tileB), Color.green, 100f);
	}

	private Vector3 CoordinatesToWorldPoint(Coordinates tile)
	{
		return new Vector3(-Width / 2f + 0.5f + tile.TileX, 2, -Height / 2f + 0.5f + tile.TileY);
	}

	private bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Height;
	}
}