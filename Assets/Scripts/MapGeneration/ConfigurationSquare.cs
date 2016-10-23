public class ConfigurationSquare
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

	public ConfigurationSquare(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
	{
		TopLeft = topLeft;
		TopRight = topRight;
		BottomRight = bottomRight;
		BottomLeft = bottomLeft;

		CenterTop = TopLeft.RightNode;
		CenterRight = BottomRight.AboveNode;
		CenterBottom = BottomLeft.RightNode;
		CenterLeft = BottomLeft.AboveNode;

		ConfigureSquare();
	}

	public override string ToString()
	{
		return Configuration.ToString();
	}

	private void ConfigureSquare()
	{
		if (BottomLeft.Active)
		{
			Configuration += 1;
		}
		if (BottomRight.Active)
		{
			Configuration += 2;
		}
		if (TopRight.Active)
		{
			Configuration += 4;
		}
		if (TopLeft.Active)
		{
			Configuration += 8;
		}
	}
}