public struct Coordinates
{
	public float X;
	public float Y;

	public Coordinates(int x, int y)
	{
		X = x;
		Y = y;
	}

	public Coordinates(float x, float y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return X + "; " + Y;
	}
}