using UnityEngine;

public class SquareGrid
{
	public Square[,] Squares;

	public SquareGrid(Tile[,] map, float squareSize)
	{
		var nodeCountX = map.GetLength(0);
		var nodeCountY = map.GetLength(1);

		ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

		for (var x = 0; x < nodeCountX; x++)
		{
			for (var y = 0; y < nodeCountY; y++)
			{
				var position = new Vector3(x * squareSize, 0, y * squareSize);
				controlNodes[x, y] = new ControlNode(position, map[x, y].Type == TileType.Floor, squareSize);
			}
		}

		Squares = new Square[nodeCountX - 1, nodeCountY - 1];
		for (var x = 0; x < nodeCountX - 1; x++)
		{
			for (var y = 0; y < nodeCountY - 1; y++)
			{
				Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
			}
		}
	}
}