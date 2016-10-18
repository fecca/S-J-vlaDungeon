public struct Coordinates
{
	public int X;
	public int Y;

	public Coordinates(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return X + "; " + Y;
	}
}