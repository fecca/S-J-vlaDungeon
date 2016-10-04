using System.Collections.Generic;

public class Room
{
	public List<Coordinates> Tiles;
	public List<Coordinates> EdgeTiles;
	public List<Room> ConnectedRooms;
	public int RoomSize;

	public Room() { }

	public Room(List<Coordinates> roomTiles, int[,] map)
	{
		Tiles = roomTiles;
		RoomSize = Tiles.Count;
		ConnectedRooms = new List<Room>();
		EdgeTiles = new List<Coordinates>();

		for (var i = 0; i < Tiles.Count; i++)
		{
			for (var x = Tiles[i].TileX - 1; x <= Tiles[i].TileX + 1; x++)
			{
				for (var y = Tiles[i].TileY - 1; y <= Tiles[i].TileY + 1; y++)
				{
					if (x == Tiles[i].TileX || y == Tiles[i].TileY)
					{
						if (map[x, y] == 1)
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
}