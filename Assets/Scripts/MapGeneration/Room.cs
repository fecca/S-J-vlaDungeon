using System;
using System.Collections.Generic;
using System.Linq;

public class Room : IComparable<Room>
{
	public List<Tile> Tiles;
	public List<Room> ConnectedRooms;
	public int RoomSize;
	public bool IsMainRoom;
	public bool IsAccessibleFromMainRoom;

	public Room(List<Tile> roomTiles)
	{
		Tiles = roomTiles;
		RoomSize = roomTiles.Count;
		ConnectedRooms = new List<Room>(64);
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