using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	[SerializeField]
	private MeshFilter Walls = null;
	[SerializeField]
	private int WallHeight = 2;
	[SerializeField]
	private int WallSegments = 1;
	[SerializeField]
	private bool RandomizeEdgeVertexPositions = false;

	private List<Vector3> _vertices = new List<Vector3>(16184);
	private List<int> _triangles = new List<int>(32768);
	//private Dictionary<int, List<Triangle>> _triangleDictionary = new Dictionary<int, List<Triangle>>(16184);
	//private List<List<int>> _outlines = new List<List<int>>(1024);
	//private HashSet<int> _checkedVertices = new HashSet<int>();

	public void GenerateMesh(Tile[,] map, float squareSize)
	{
		//var squareGrid = new SquareGrid(map, squareSize);
		_vertices.Clear();
		_triangles.Clear();
		//_triangleDictionary.Clear();
		//_outlines.Clear();
		//_checkedVertices.Clear();

		for (var x = 0; x < map.GetLength(0) - 1; x++)
		{
			for (var y = 0; y < map.GetLength(1) - 1; y++)
			{
				TriangulateSquare(map[x, y]);
			}
		}

		CreateMesh();

		CreateWallMesh(map);
	}

	private void CreateMesh()
	{
		var mesh = new Mesh();
		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		mesh.RecalculateNormals();

		var meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
	}

	private void TriangulateSquare(Tile tile)
	{
		var square = tile.ConfigurationSquare;
		switch (square.Configuration)
		{
			// 0 points
			case 0:
				break;

			// 1 points
			case 1:
				tile.AddVertices(square.CenterBottom.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterLeft, square.CenterBottom);
				break;
			case 2:
				tile.AddVertices(square.CenterRight.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;
			case 4:
				tile.AddVertices(square.CenterTop.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;
			case 8:
				tile.AddVertices(square.CenterLeft.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 points
			case 3:
				tile.AddVertices(square.CenterRight.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterLeft, square.CenterRight, square.BottomRight);
				break;
			case 6:
				tile.AddVertices(square.CenterTop.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterTop, square.TopRight);
				break;
			case 12:
				tile.AddVertices(square.CenterLeft.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopRight, square.CenterRight, square.CenterLeft, square.TopLeft);
				break;
			case 9:
				tile.AddVertices(square.CenterBottom.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;
			case 5:
				tile.AddVertices(square.CenterRight.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.BottomLeft, square.CenterBottom, square.CenterRight, square.TopRight, square.CenterTop, square.CenterLeft);
				break;
			case 10:
				tile.AddVertices(square.CenterTop.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.BottomRight, square.CenterRight, square.CenterTop, square.TopLeft, square.CenterLeft, square.CenterBottom);
				break;

			// 3 points
			case 11:
				tile.AddVertices(square.CenterRight.Position, square.CenterTop.Position);
				BuildmeshFromPoints(square.BottomLeft, square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight);
				break;
			case 7:
				tile.AddVertices(square.CenterTop.Position, square.CenterLeft.Position);
				BuildmeshFromPoints(square.BottomRight, square.BottomLeft, square.CenterLeft, square.CenterTop, square.TopRight);
				break;
			case 14:
				tile.AddVertices(square.CenterLeft.Position, square.CenterBottom.Position);
				BuildmeshFromPoints(square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft, square.TopLeft);
				break;
			case 13:
				tile.AddVertices(square.CenterBottom.Position, square.CenterRight.Position);
				BuildmeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
				break;

			// 4 points
			case 15:
				BuildmeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				//_checkedVertices.Add(square.TopLeft.VertexIndex);
				//_checkedVertices.Add(square.TopRight.VertexIndex);
				//_checkedVertices.Add(square.BottomRight.VertexIndex);
				//_checkedVertices.Add(square.BottomLeft.VertexIndex);
				break;
		}
	}

	private void BuildmeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if (points.Length == 3)
		{
			CreateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length == 4)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length == 5)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
			CreateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length == 6)
		{
			CreateTriangle(points[0], points[1], points[2]);
			CreateTriangle(points[0], points[2], points[3]);
			CreateTriangle(points[0], points[3], points[4]);
			CreateTriangle(points[0], points[4], points[5]);
		}
	}

	private void AssignVertices(Node[] points)
	{
		for (var i = 0; i < points.Length; i++)
		{
			var point = points[i];
			if (point.VertexIndex == -1)
			{
				point.VertexIndex = _vertices.Count;

				if (RandomizeEdgeVertexPositions)
				{
					point.Position += Vector3.right * Random.Range(-0.1f, 0.1f);
					point.Position += Vector3.forward * Random.Range(-0.1f, 0.1f);
				}

				_vertices.Add(point.Position);
			}
		}
	}

	private void CreateTriangle(Node a, Node b, Node c)
	{
		_triangles.Add(a.VertexIndex);
		_triangles.Add(b.VertexIndex);
		_triangles.Add(c.VertexIndex);

		//Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);

		//AddTriangleToDictionary(triangle.VertexIndexA, triangle);
		//AddTriangleToDictionary(triangle.VertexIndexB, triangle);
		//AddTriangleToDictionary(triangle.VertexIndexC, triangle);
	}

	private void CreateWallMesh(Tile[,] map)
	{
		//CalculateMeshOutlines();

		//var wallVertices = new List<Vector3>(8092);
		//var wallTriangles = new List<int>(4096);
		//var wallMesh = new Mesh();
		//var SegmentHeight = (float)WallHeight / WallSegments;

		//for (var i = 0; i < _outlines.Count; i++)
		//{
		//	var outline = _outlines[i];
		//for (var j = 0; j < outline.Count - 1; j++)
		//{
		//	var topRight = _vertices[outline[j]];
		//	var topLeft = _vertices[outline[j + 1]];
		//	var bottomRight = topRight;
		//	var bottomLeft = topLeft;

		//	for (var k = 0; k < WallSegments; k++)
		//	{
		//		topRight = bottomRight;
		//		topLeft = bottomLeft;
		//		bottomRight += Vector3.up * SegmentHeight;
		//		bottomLeft += Vector3.up * SegmentHeight;

		//		wallVertices.Add(topRight);
		//		wallTriangles.Add(wallVertices.Count - 1);
		//		wallVertices.Add(bottomRight);
		//		wallTriangles.Add(wallVertices.Count - 1);
		//		wallVertices.Add(bottomLeft);
		//		wallTriangles.Add(wallVertices.Count - 1);

		//		wallVertices.Add(bottomLeft);
		//		wallTriangles.Add(wallVertices.Count - 1);
		//		wallVertices.Add(topLeft);
		//		wallTriangles.Add(wallVertices.Count - 1);
		//		wallVertices.Add(topRight);
		//		wallTriangles.Add(wallVertices.Count - 1);
		//	}
		//}
		//}

		var wallVertices = new List<Vector3>(8092);
		var wallTriangles = new List<int>(4096);
		var wallMesh = new Mesh();

		for (var x = 0; x < map.GetLength(0); x++)
		{
			for (var y = 0; y < map.GetLength(1); y++)
			{
				var tile = map[x, y];
				if (tile.CoreVertices.Count > 0)
				{
					var randomHeight = Vector3.up * Random.Range(0.9f, 1.1f);
					wallVertices.Add(tile.CoreVertices[0]);
					wallVertices.Add(tile.CoreVertices[1]);
					wallVertices.Add(tile.CoreVertices[1] + randomHeight);

					wallTriangles.Add(wallVertices.Count - 3);
					wallTriangles.Add(wallVertices.Count - 2);
					wallTriangles.Add(wallVertices.Count - 1);

					wallVertices.Add(tile.CoreVertices[1] + randomHeight);
					wallVertices.Add(tile.CoreVertices[0] + Vector3.up * Random.Range(0.9f, 1.1f));
					wallVertices.Add(tile.CoreVertices[0]);

					wallTriangles.Add(wallVertices.Count - 3);
					wallTriangles.Add(wallVertices.Count - 2);
					wallTriangles.Add(wallVertices.Count - 1);
				}
			}
		}

		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		Walls.mesh = wallMesh;

		//var collider = gameObject.GetComponent<MeshCollider>();
		//collider.sharedMesh = wallMesh;
	}

	//private void CalculateMeshOutlines()
	//{
	//	for (var vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
	//	{
	//		if (!_checkedVertices.Contains(vertexIndex))
	//		{
	//			var newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
	//			if (newOutlineVertex != -1)
	//			{
	//				_checkedVertices.Add(vertexIndex);
	//				_outlines.Add(new List<int>() { vertexIndex });
	//				FollowOutline(newOutlineVertex, _outlines.Count - 1);
	//				_outlines[_outlines.Count - 1].Add(vertexIndex);
	//			}
	//		}
	//	}
	//}

	//private int GetConnectedOutlineVertex(int vertexIndex)
	//{
	//	var trianglesContainingVertex = _triangleDictionary[vertexIndex];
	//	for (var i = 0; i < trianglesContainingVertex.Count; i++)
	//	{
	//		var triangle = trianglesContainingVertex[i];
	//		for (var j = 0; j < 3; j++)
	//		{
	//			var vertexB = triangle[j];
	//			if (vertexB != vertexIndex && !_checkedVertices.Contains(vertexB))
	//			{
	//				if (IsOutlineEdge(vertexIndex, vertexB))
	//				{
	//					return vertexB;
	//				}
	//			}
	//		}
	//	}

	//	return -1;
	//}

	//private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
	//{
	//	if (_triangleDictionary.ContainsKey(vertexIndexKey))
	//	{
	//		_triangleDictionary[vertexIndexKey].Add(triangle);
	//	}
	//	else
	//	{
	//		_triangleDictionary.Add(vertexIndexKey, new List<Triangle>(16) { triangle });
	//	}
	//}

	//private bool IsOutlineEdge(int vertexA, int vertexB)
	//{
	//	var trianglesContainingVertexA = _triangleDictionary[vertexA];
	//	var sharedTriangleCount = 0;
	//	for (var i = 0; i < trianglesContainingVertexA.Count; i++)
	//	{
	//		if (trianglesContainingVertexA[i].Contains(vertexB))
	//		{
	//			sharedTriangleCount++;
	//			if (sharedTriangleCount > 1)
	//			{
	//				break;
	//			}
	//		}
	//	}

	//	return sharedTriangleCount == 1;
	//}

	//private void FollowOutline(int vertexIndex, int outlineIndex)
	//{
	//	_outlines[outlineIndex].Add(vertexIndex);
	//	_checkedVertices.Add(vertexIndex);
	//	var nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
	//	if (nextVertexIndex != -1)
	//	{
	//		FollowOutline(nextVertexIndex, outlineIndex);
	//	}
	//}
}