public struct Coordinates
{
	public float X { get; private set; }
	public float Y { get; private set; }

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