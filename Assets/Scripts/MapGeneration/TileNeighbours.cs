public class TileNeighbours
{
	public Tile TopLeft;
	public Tile Top;
	public Tile TopRight;
	public Tile Right;
	public Tile BottomRight;
	public Tile Bottom;
	public Tile BottomLeft;
	public Tile Left;

	public TileNeighbours(
		Tile topLeftNeighbour,
		Tile topNeighbour,
		Tile topRightNeighbour,
		Tile rightNeighbour,
		Tile bottomRightNeighbour,
		Tile bottomNeighbour,
		Tile bottomLeftNeighbour,
		Tile leftNeighbour)
	{
		TopLeft = topLeftNeighbour;
		Top = topNeighbour;
		TopRight = topRightNeighbour;
		Right = rightNeighbour;
		BottomRight = bottomRightNeighbour;
		Bottom = bottomNeighbour;
		BottomLeft = bottomLeftNeighbour;
		Left = leftNeighbour;
	}
}