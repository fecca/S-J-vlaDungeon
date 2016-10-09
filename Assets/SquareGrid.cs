using UnityEngine;

public class SquareGrid
{
	public Square[,] Squares;

	public SquareGrid(int[,] map, float squareSize)
	{
		int nodeCountX = map.GetLength(0);
		int nodeCountY = map.GetLength(1);
		float mapWidth = nodeCountX * squareSize;
		float mapHeight = nodeCountY * squareSize;

		ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

		for (var x = 0; x < nodeCountX; x++)
		{
			for (var y = 0; y < nodeCountY; y++)
			{
				var position = new Vector3(x * squareSize, 0, y * squareSize);
				controlNodes[x, y] = new ControlNode(position, map[x, y] == 1, squareSize);
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