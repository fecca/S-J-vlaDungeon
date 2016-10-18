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
	private List<Room> _survivingRooms = new List<Room>(128);

	public Tile[,] GenerateMap()
	{
		_map = new Tile[Width, Height];

		RandomFillMap();
		GenerateClusters();
		FilterWalls();
		FilterRooms();
		ConnectClosestRooms();
		ConfigureTiles();

		return _map;
	}
	public Vector3 GetPlayerPosition()
	{
		var room = _survivingRooms.First();
		var tile = room.Tiles.GetRandomElement();
		while (!tile.IsWalkable)
		{
			tile = room.Tiles.GetRandomElement();
		}
		var position = new Vector3(tile.WorldCoordinates.X, 5, tile.WorldCoordinates.Y);

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
				if (tile.IsWalkable)
				{
					walkableTiles.Add(tile);
				}
			}
		}

		return walkableTiles;
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
					wallRegion[j].Type = TileType.Floor;
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
					roomRegion[j].Type = TileType.Wall;
				}
			}
			else
			{
				_survivingRooms.Add(new Room(roomRegion, _map));
			}
		}

		_survivingRooms.Sort();
	}
	private void ConnectClosestRooms(bool forceAccessibilityFromMainRoom = false)
	{
		_survivingRooms[0].IsMainRoom = true;
		_survivingRooms[0].IsAccessibleFromMainRoom = true;

		var roomListA = new List<Room>(64);
		var roomListB = new List<Room>(64);

		if (forceAccessibilityFromMainRoom)
		{
			for (var i = 0; i < _survivingRooms.Count; i++)
			{
				if (_survivingRooms[i].IsAccessibleFromMainRoom)
				{
					roomListB.Add(_survivingRooms[i]);
				}
				else
				{
					roomListA.Add(_survivingRooms[i]);
				}
			}
		}
		else
		{
			roomListA = _survivingRooms;
			roomListB = _survivingRooms;
		}

		var bestDistance = 0f;
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

				for (var tileIndexA = 0; tileIndexA < roomListA[i].Tiles.Count; tileIndexA++)
				{
					for (var tileIndexB = 0; tileIndexB < roomListB[j].Tiles.Count; tileIndexB++)
					{
						var tileA = roomListA[i].Tiles[tileIndexA];
						var tileB = roomListB[j].Tiles[tileIndexB];
						var distanceX = (tileA.WorldCoordinates.X - tileB.WorldCoordinates.X) * (tileA.WorldCoordinates.X - tileB.WorldCoordinates.X);
						var distanceY = (tileA.WorldCoordinates.Y - tileB.WorldCoordinates.Y) * (tileA.WorldCoordinates.Y - tileB.WorldCoordinates.Y);
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
			ConnectClosestRooms(true);
		}

		if (!forceAccessibilityFromMainRoom)
		{
			ConnectClosestRooms(true);
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

		var line = GetLine(tileA.WorldCoordinates, tileB.WorldCoordinates);
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
					var drawX = (int)c.X + x;
					var drawY = (int)c.Y + y;
					if (IsInMapRange(drawX, drawY))
					{
						_map[drawX, drawY].Type = TileType.Floor;
					}
				}
			}
		}
	}

	private bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Height;
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
					var regionTiles = GetRegionTiles(x, y);
					regions.Add(regionTiles);

					for (var i = 0; i < regionTiles.Count; i++)
					{
						var xRegion = (int)regionTiles[i].GridCoordinates.X;
						var yRegion = (int)regionTiles[i].GridCoordinates.Y;
						mapFlags[xRegion, yRegion] = true;
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

			for (var x = (int)tile.GridCoordinates.X - 1; x <= tile.GridCoordinates.X + 1; x++)
			{
				for (var y = (int)tile.GridCoordinates.Y - 1; y <= tile.GridCoordinates.Y + 1; y++)
				{
					if (IsInMapRange(x, y) && (y == tile.GridCoordinates.Y || x == tile.GridCoordinates.X))
					{
						if (!mapFlags[x, y] && _map[x, y].Type == tileType)
						{
							mapFlags[x, y] = true;
							queue.Enqueue(_map[x, y]);
						}
					}
				}
			}
		}

		return tiles;
	}
	private List<Coordinates> GetLine(Coordinates from, Coordinates to)
	{
		Debug.DrawLine(new Vector3(from.X, 1f, from.Y), new Vector3(to.X, 1f, to.Y), Color.yellow, 100f);
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
	private void ConfigureTiles()
	{
		var controlNodes = new ControlNode[Width, Height];
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				var tile = _map[x, y];
				var position = new Vector3(tile.WorldCoordinates.X, 0, tile.WorldCoordinates.Y);
				var active = tile.Type == TileType.Floor;
				controlNodes[x, y] = new ControlNode(position, active);
			}
		}

		for (var x = 0; x < Width - 1; x++)
		{
			for (var y = 0; y < Height - 1; y++)
			{
				var tile = _map[x, y];
				tile.SetConfiguration(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
			}
		}
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

		foreach (Tile tile in _map)
		{
			Gizmos.color = tile.Type == TileType.Floor ? Color.green : Color.red;
			Gizmos.DrawCube(new Vector3(tile.WorldCoordinates.X, 0, tile.WorldCoordinates.Y), Vector3.one * 0.2f);
		}

		//foreach (Room room in _survivingRooms)
		//{
		//	foreach (Tile tile in room.Tiles)
		//	{
		//		Gizmos.color = Color.green;
		//		Gizmos.DrawCube(new Vector3(tile.WorldCoordinates.X, 0, tile.WorldCoordinates.Y), Vector3.one * 0.2f);
		//	}
		//}
	}
}