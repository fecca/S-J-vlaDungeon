public class Tile
{
	public Coordinates Coordinates;
	public TileType Type;

	public Tile(int x, int y, TileType type)
	{
		Coordinates = new Coordinates(x, y);
		Type = type;
	}
}