public class Square
{
	public ControlNode TopLeft;
	public ControlNode TopRight;
	public ControlNode BottomRight;
	public ControlNode BottomLeft;
	public Node CenterTop;
	public Node CenterRight;
	public Node CenterBottom;
	public Node CenterLeft;
	public int Configuration;

	public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
	{
		TopLeft = topLeft;
		TopRight = topRight;
		BottomRight = bottomRight;
		BottomLeft = bottomLeft;

		CenterTop = TopLeft.RightNode;
		CenterRight = BottomRight.AboveNode;
		CenterBottom = BottomLeft.RightNode;
		CenterLeft = BottomLeft.AboveNode;

		if (TopLeft.Active)
		{
			Configuration += 8;
		}
		if (TopRight.Active)
		{
			Configuration += 4;
		}
		if (BottomRight.Active)
		{
			Configuration += 2;
		}
		if (BottomLeft.Active)
		{
			Configuration += 1;
		}
	}
}