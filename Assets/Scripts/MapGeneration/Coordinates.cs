public struct Coordinates
{
	public int X;
	public int Y;

	public Coordinates(int tileX, int tileY)
	{
		X = tileX;
		Y = tileY;
	}

	public override string ToString()
	{
		return X + "; " + Y;
	}
}