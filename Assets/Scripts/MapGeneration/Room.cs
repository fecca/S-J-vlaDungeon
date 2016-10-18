using System;
using System.Collections.Generic;
using System.Linq;

public class Room : IComparable<Room>
{
	public List<Tile> Tiles;
	//public List<Tile> EdgeTiles;
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
		//EdgeTiles = new List<Tile>(1024);
		//EdgeTiles = Tiles.Where(p => p.Type == TileType.Floor).ToList();

		//for (var i = 0; i < Tiles.Count; i++)
		//{
		//for (var x = Tiles[i].GridCoordinates.X - 1; x <= Tiles[i].GridCoordinates.X + 1; x++)
		//{
		//	for (var y = Tiles[i].GridCoordinates.Y - 1; y <= Tiles[i].GridCoordinates.Y + 1; y++)
		//	{
		//		if (x == Tiles[i].GridCoordinates.X || y == Tiles[i].GridCoordinates.Y)
		//{
		//if (Tiles[i].Type == TileType.Floor)
		//{
		//	EdgeTiles.Add(Tiles[i]);
		//}
		//		}
		//	}
		//}
		//}
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