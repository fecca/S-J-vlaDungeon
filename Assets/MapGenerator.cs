using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	private bool DrawGizmos = false;
	[SerializeField]
	private string Seed = "Seed";
	[SerializeField]
	private bool UseRandomSeed = true;
	[SerializeField]
	private int Width = 64;
	[SerializeField]
	private int Height = 64;
	[SerializeField]
	[Range(45, 55)]
	private int RandomFillPercent = 50;
	[SerializeField]
	private int WallThresholdSize = 50;
	[SerializeField]
	private int RoomThresholdSize = 50;
	[SerializeField]
	private int CorridorThickness = 2;
	[SerializeField]
	private int SmoothingLoops = 5;

	private Tile[,] _map;
	private List<Room> SurvivingRooms = new List<Room>(128);

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GenerateMap();
		}
	}

	public Tile[,] GenerateMap()
	{
		_map = new Tile[Width, Height];

		RandomFillMap();
		GenerateClusters();
		FilterWalls();
		FilterRooms();
		ConnectClosestRooms(SurvivingRooms);
		CreateSquares();

		return _map;
	}

	private void CreateSquares()
	{
		var controlNodes = new ControlNode[Width, Height];
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				var position = new Vector3(x * Constants.TileSize - Constants.TileSize / 2f, 0, y * Constants.TileSize - Constants.TileSize / 2f);
				controlNodes[x, y] = new ControlNode(position, _map[x, y].Type == TileType.Floor);
			}
		}

		for (var x = 0; x < Width - 1; x++)
		{
			for (var y = 0; y < Height - 1; y++)
			{
				_map[x, y].ConfigurationSquare = new ConfigurationSquare(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
			}
		}
	}

	private void RandomFillMap()
	{
		if (UseRandomSeed)
		{
			Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
		}

		System.Random rng = new System.Random(Seed.GetHashCode());
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
				{
					_map[x, y] = new Tile(x, y, TileType.Wall);
				}
				else
				{
					_map[x, y] = new Tile(x, y, rng.Next(0, 100) < RandomFillPercent ? TileType.Floor : TileType.Wall);
				}
			}
		}
	}

	private void GenerateClusters()
	{
		for (var i = 0; i < SmoothingLoops; i++)
		{
			for (var x = 1; x < Width - 1; x++)
			{
				for (var y = 1; y < Height - 1; y++)
				{
					if (GetNumberOfNeighbouringTiles(x, y) > 4)
					{
						_map[x, y].Type = TileType.Floor;
					}
					else if (GetNumberOfNeighbouringTiles(x, y) < 4)
					{
						_map[x, y].Type = TileType.Wall;
					}
				}
			}
		}
	}

	private int GetNumberOfNeighbouringTiles(int xPosition, int yPosition)
	{
		var neighbouringTiles = 0;
		for (var x = xPosition - 1; x <= xPosition + 1; x++)
		{
			for (var y = yPosition - 1; y <= yPosition + 1; y++)
			{
				if (IsInMapRange(x, y))
				{
					if (x != xPosition || y != yPosition)
					{
						neighbouringTiles += _map[x, y].Type == TileType.Floor ? 1 : 0;
					}
				}
			}
		}

		return neighbouringTiles;
	}

	private void FilterWalls()
	{
		var wallRegions = GetRegions(TileType.Wall);
		for (var i = 0; i < wallRegions.Count; i++)
		{
			var wallRegion = wallRegions[i];
			if (wallRegion.Count < WallThresholdSize)
			{
				for (var j = 0; j < wallRegion.Count; j++)
				{
					_map[wallRegion[j].Coordinates.X, wallRegion[j].Coordinates.Y].Type = TileType.Floor;
				}
			}
		}
	}

	private void FilterRooms()
	{
		var roomRegions = GetRegions(TileType.Floor);
		for (var i = 0; i < roomRegions.Count; i++)
		{
			var roomRegion = roomRegions[i];
			if (roomRegion.Count < RoomThresholdSize)
			{
				for (var j = 0; j < roomRegion.Count; j++)
				{
					_map[roomRegion[j].Coordinates.X, roomRegion[j].Coordinates.Y].Type = TileType.Wall;
				}
			}
			else
			{
				SurvivingRooms.Add(new Room(roomRegion, _map));
			}
		}

		SurvivingRooms.Sort();
	}

	private List<List<Tile>> GetRegions(TileType tileType)
	{
		var regions = new List<List<Tile>>(64);
		var mapFlags = new bool[Width, Height];

		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				if (!mapFlags[x, y] && _map[x, y].Type == tileType)
				{
					var newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					for (var i = 0; i < newRegion.Count; i++)
					{
						mapFlags[newRegion[i].Coordinates.X, newRegion[i].Coordinates.Y] = true;
					}
				}
			}
		}

		return regions;
	}

	private List<Tile> GetRegionTiles(int startX, int startY)
	{
		var tiles = new List<Tile>(1024);
		var mapFlags = new bool[Width, Height];
		var tileType = _map[startX, startY].Type;
		var queue = new Queue<Tile>();

		queue.Enqueue(new Tile(startX, startY, tileType));
		mapFlags[startX, startY] = true;

		while (queue.Count > 0)
		{
			var tile = queue.Dequeue();
			tiles.Add(tile);

			for (var x = tile.Coordinates.X - 1; x <= tile.Coordinates.X + 1; x++)
			{
				for (var y = tile.Coordinates.Y - 1; y <= tile.Coordinates.Y + 1; y++)
				{
					if (IsInMapRange(x, y) && (y == tile.Coordinates.Y || x == tile.Coordinates.X))
					{
						if (!mapFlags[x, y] && _map[x, y].Type == tileType)
						{
							mapFlags[x, y] = true;
							queue.Enqueue(new Tile(x, y, tileType));
						}
					}
				}
			}
		}

		return tiles;
	}

	private void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
	{
		allRooms[0].IsMainRoom = true;
		allRooms[0].IsAccessibleFromMainRoom = true;

		var roomListA = new List<Room>(64);
		var roomListB = new List<Room>(64);

		if (forceAccessibilityFromMainRoom)
		{
			for (var i = 0; i < allRooms.Count; i++)
			{
				if (allRooms[i].IsAccessibleFromMainRoom)
				{
					roomListB.Add(allRooms[i]);
				}
				else
				{
					roomListA.Add(allRooms[i]);
				}
			}
		}
		else
		{
			roomListA = allRooms;
			roomListB = allRooms;
		}

		var bestDistance = 0;
		var possibleConnectionFound = false;
		Tile bestTileA = null;
		Tile bestTileB = null;
		Room bestRoomA = null;
		Room bestRoomB = null;

		for (var i = 0; i < roomListA.Count; i++)
		{
			if (!forceAccessibilityFromMainRoom)
			{
				possibleConnectionFound = false;
				if (roomListA[i].ConnectedRooms.Count > 0)
				{
					continue;
				}
			}

			for (var j = 0; j < roomListB.Count; j++)
			{
				if (roomListA[i] == roomListB[j] || roomListA[i].IsConnected(roomListB[j]))
				{
					continue;
				}

				for (var tileIndexA = 0; tileIndexA < roomListA[i].EdgeTiles.Count; tileIndexA++)
				{
					for (var tileIndexB = 0; tileIndexB < roomListB[j].EdgeTiles.Count; tileIndexB++)
					{
						var tileA = roomListA[i].EdgeTiles[tileIndexA];
						var tileB = roomListB[j].EdgeTiles[tileIndexB];
						var distanceX = (tileA.Coordinates.X - tileB.Coordinates.X) * (tileA.Coordinates.X - tileB.Coordinates.X);
						var distanceY = (tileA.Coordinates.Y - tileB.Coordinates.Y) * (tileA.Coordinates.Y - tileB.Coordinates.Y);
						var distanceBetweenRooms = distanceX + distanceY;

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
						{
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomListA[i];
							bestRoomB = roomListB[j];
						}
					}
				}
			}

			if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom)
		{
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom)
		{
			ConnectClosestRooms(allRooms, true);
		}
	}

	private void CreatePassage(Room roomA, Room roomB, Tile tileA, Tile tileB)
	{
		if (roomA.IsAccessibleFromMainRoom)
		{
			roomB.SetAccessibleFromMainRoom();
		}
		else if (roomB.IsAccessibleFromMainRoom)
		{
			roomA.SetAccessibleFromMainRoom();
		}
		roomA.ConnectedRooms.Add(roomB);
		roomB.ConnectedRooms.Add(roomA);

		var line = GetLine(tileA.Coordinates, tileB.Coordinates);
		for (var i = 0; i < line.Count; i++)
		{
			CreateCorridor(line[i], CorridorThickness);
		}
	}

	private void CreateCorridor(Coordinates c, int r)
	{
		for (var x = -r; x <= r; x++)
		{
			for (var y = -r; y <= r; y++)
			{
				if (x * x + y * y <= r * r)
				{
					var drawX = c.X + x;
					var drawY = c.Y + y;
					if (IsInMapRange(drawX, drawY))
					{
						_map[drawX, drawY].Type = TileType.Floor;
					}
				}
			}
		}
	}

	private List<Coordinates> GetLine(Coordinates from, Coordinates to)
	{
		var line = new List<Coordinates>(64);

		var x = from.X;
		var y = from.Y;

		var deltaX = to.X - from.X;
		var deltaY = to.Y - from.Y;

		var step = Math.Sign(deltaX);
		var gradientStep = Math.Sign(deltaY);

		var longest = Mathf.Abs(deltaX);
		var shortest = Mathf.Abs(deltaY);

		var inverted = false;
		if (longest < shortest)
		{
			inverted = true;
			longest = Mathf.Abs(deltaY);
			shortest = Mathf.Abs(deltaX);
			step = Math.Sign(deltaY);
			gradientStep = Math.Sign(deltaX);
		}

		var gradientAccumulation = longest / 2;
		for (var i = 0; i < longest; i++)
		{
			line.Add(new Coordinates(x, y));
			if (inverted)
			{
				y += step;
			}
			else
			{
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest)
			{
				if (inverted)
				{
					x += gradientStep;
				}
				else
				{
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	private bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Height;
	}

	public Vector3 GetPlayerPosition()
	{
		var room = SurvivingRooms.First();
		var tile = room.Tiles.GetRandomElement();
		var position = new Vector3(tile.Coordinates.X * Constants.TileSize, 0, tile.Coordinates.Y * Constants.TileSize);

		return position;
	}

	public List<Tile> GetWalkableTiles()
	{
		var walkableTiles = new List<Tile>();
		for (var x = 0; x < _map.GetLength(0); x++)
		{
			for (var y = 0; y < _map.GetLength(1); y++)
			{
				var tile = _map[x, y];
				if (tile.Type == TileType.Floor && !tile.IsConfigured)
				{
					walkableTiles.Add(tile);
				}
			}
		}

		return walkableTiles;
	}

	private void OnDrawGizmos()
	{
		if (!DrawGizmos)
		{
			return;
		}

		if (_map == null)
		{
			return;
		}

		for (var x = 0; x < _map.GetLength(0); x++)
		{
			for (var y = 0; y < _map.GetLength(1); y++)
			{
				Gizmos.color = _map[x, y].Type == TileType.Floor ? Color.white : Color.gray;
				if (x == 1 && y == 19)
				{
					Gizmos.color = Color.red;
				}
				Gizmos.DrawCube(new Vector3(x * Constants.TileSize, 0, y * Constants.TileSize), Vector3.one * 0.2f);
			}
		}
	}
}