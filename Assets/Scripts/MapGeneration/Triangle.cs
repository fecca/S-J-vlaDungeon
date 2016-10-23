public struct Triangle
{
	private int[] _vertices;

	public int VertexIndexA;
	public int VertexIndexB;
	public int VertexIndexC;

	public Triangle(int a, int b, int c)
	{
		VertexIndexA = a;
		VertexIndexB = b;
		VertexIndexC = c;

		_vertices = new int[3];
		_vertices[0] = a;
		_vertices[1] = b;
		_vertices[2] = c;
	}

	public int this[int i]
	{
		get
		{
			return _vertices[i];
		}
	}
	public bool Contains(int vertexIndex)
	{
		return vertexIndex == VertexIndexA || vertexIndex == VertexIndexB || vertexIndex == VertexIndexC;
	}
}