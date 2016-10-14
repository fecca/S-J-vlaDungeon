using System;
using System.Collections.Generic;

public class Room : IComparable<Room>
{
	public List<Tile> Tiles;
	public List<Tile> EdgeTiles;
	public List<Room> ConnectedRooms;
	public int RoomSize;
	public bool IsMainRoom;
	public bool IsAccessibleFromMainRoom;

	public Room() { }

	public Room(List<Tile> roomTiles, Tile[,] map)
	{
		Tiles = roomTiles;
		RoomSize = Tiles.Count;
		ConnectedRooms = new List<Room>(64);
		EdgeTiles = new List<Tile>(1024);

		for (var i = 0; i < Tiles.Count; i++)
		{
			for (var x = Tiles[i].Coordinates.X - 1; x <= Tiles[i].Coordinates.X + 1; x++)
			{
				for (var y = Tiles[i].Coordinates.Y - 1; y <= Tiles[i].Coordinates.Y + 1; y++)
				{
					if (x == Tiles[i].Coordinates.X || y == Tiles[i].Coordinates.Y)
					{
						if (map[x, y].Type == TileType.Floor)
						{
							EdgeTiles.Add(Tiles[i]);
						}
					}
				}
			}
		}
	}

	public bool IsConnected(Room otherRoom)
	{
		return ConnectedRooms.Contains(otherRoom);
	}

	public void SetAccessibleFromMainRoom()
	{
		if (!IsAccessibleFromMainRoom)
		{
			IsAccessibleFromMainRoom = true;
			for (var i = 0; i < ConnectedRooms.Count; i++)
			{
				ConnectedRooms[i].IsAccessibleFromMainRoom = true;
			}
		}
	}

	public int CompareTo(Room otherRoom)
	{
		return otherRoom.RoomSize.CompareTo(RoomSize);
	}
}