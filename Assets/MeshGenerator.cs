﻿using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	[SerializeField]
	private MeshFilter Walls;

	private SquareGrid _squareGrid;
	private List<Vector3> _vertices;
	private List<int> _triangles;
	private Dictionary<int, List<Triangle>> _triangleDictionary;
	private List<List<int>> _outlines;
	private HashSet<int> _checkedVertices;

	public void GenerateMesh(int[,] map, float squareSize)
	{
		_squareGrid = new SquareGrid(map, squareSize);
		_vertices = new List<Vector3>();
		_triangles = new List<int>();
		_triangleDictionary = new Dictionary<int, List<Triangle>>();
		_outlines = new List<List<int>>();
		_checkedVertices = new HashSet<int>();

		for (var x = 0; x < _squareGrid.Squares.GetLength(0); x++)
		{
			for (var y = 0; y < _squareGrid.Squares.GetLength(1); y++)
			{
				TriangulateSquare(_squareGrid.Squares[x, y]);
			}
		}

		var mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		mesh.RecalculateNormals();

		CreateWallMesh();
	}

	private void TriangulateSquare(Square square)
	{
		switch (square.Configuration)
		{
			case 0:
				break;

			// 1 point
			case 1:
				MeshFromPoints(square.CenterLeft, square.CenterBottom, square.BottomLeft);
				break;
			case 2:
				MeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;
			case 4:
				MeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;
			case 8:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 point
			case 3:
				MeshFromPoints(square.CenterRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
				break;
			case 6:
				MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.CenterBottom);
				break;
			case 9:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;
			case 12:
				MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterLeft);
				break;
			case 5:
				MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft, square.CenterLeft);
				break;
			case 10:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
				break;

			// 3 point
			case 7:
				MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
				break;
			case 11:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.BottomLeft);
				break;
			case 13:
				MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
				break;
			case 14:
				MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
				break;

			// 4 points
			case 15:
				MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				_checkedVertices.Add(square.TopLeft.VertexIndex);
				_checkedVertices.Add(square.TopRight.VertexIndex);
				_checkedVertices.Add(square.BottomRight.VertexIndex);
				_checkedVertices.Add(square.BottomLeft.VertexIndex);
				break;
		}
	}

	private void MeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if (points.Length >= 3)
		{
			CreateTriangles(points[0], points[1], points[2]);
		}
		if (points.Length >= 4)
		{
			CreateTriangles(points[0], points[2], points[3]);
		}
		if (points.Length >= 5)
		{
			CreateTriangles(points[0], points[3], points[4]);
		}
		if (points.Length >= 6)
		{
			CreateTriangles(points[0], points[4], points[5]);
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
				_vertices.Add(point.Position);
			}
		}
	}

	private void CreateTriangles(Node a, Node b, Node c)
	{
		_triangles.Add(a.VertexIndex);
		_triangles.Add(b.VertexIndex);
		_triangles.Add(c.VertexIndex);

		Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
		AddTriangleToDictionary(triangle.VertexIndexA, triangle);
		AddTriangleToDictionary(triangle.VertexIndexB, triangle);
		AddTriangleToDictionary(triangle.VertexIndexC, triangle);
	}

	private void CreateWallMesh()
	{
		CalculateMeshOutlines();

		var wallVertices = new List<Vector3>();
		var wallTriangles = new List<int>();
		var wallMesh = new Mesh();
		var wallheight = 5f;

		for (var i = 0; i < _outlines.Count; i++)
		{
			var outline = _outlines[i];
			for (var j = 0; j < outline.Count - 1; j++)
			{
				var startIndex = wallVertices.Count;
				//wallVertices.Add(_vertices[outline[j]]);
				//wallVertices.Add(_vertices[outline[j + 1]]);
				//wallVertices.Add(_vertices[outline[j]] - Vector3.up * wallheight);
				//wallVertices.Add(_vertices[outline[j + 1]] - Vector3.up * wallheight);

				wallVertices.Add(_vertices[outline[j]]);
				wallTriangles.Add(wallVertices.Count - 1);
				wallVertices.Add(_vertices[outline[j]] - Vector3.up * wallheight);
				wallTriangles.Add(wallVertices.Count - 1);
				wallVertices.Add(_vertices[outline[j + 1]] - Vector3.up * wallheight);
				wallTriangles.Add(wallVertices.Count - 1);

				wallVertices.Add(_vertices[outline[j + 1]] - Vector3.up * wallheight);
				wallTriangles.Add(wallVertices.Count - 1);
				wallVertices.Add(_vertices[outline[j + 1]]);
				wallTriangles.Add(wallVertices.Count - 1);
				wallVertices.Add(_vertices[outline[j]]);
				wallTriangles.Add(wallVertices.Count - 1);
			}
		}

		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		Walls.mesh = wallMesh;
	}

	private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
	{
		if (_triangleDictionary.ContainsKey(vertexIndexKey))
		{
			_triangleDictionary[vertexIndexKey].Add(triangle);
		}
		else
		{
			_triangleDictionary.Add(vertexIndexKey, new List<Triangle> { triangle });
		}
	}

	private bool IsOutlineEdge(int vertexA, int vertexB)
	{
		var trianglesContainingVertexA = _triangleDictionary[vertexA];
		var sharedTriangleCount = 0;
		for (var i = 0; i < trianglesContainingVertexA.Count; i++)
		{
			if (trianglesContainingVertexA[i].Contains(vertexB))
			{
				sharedTriangleCount++;
				if (sharedTriangleCount > 1)
				{
					break;
				}
			}
		}

		return sharedTriangleCount == 1;
	}

	private int GetConnectedOutlineVertex(int vertexIndex)
	{
		var trianglesContainingVertex = _triangleDictionary[vertexIndex];
		for (var i = 0; i < trianglesContainingVertex.Count; i++)
		{
			var triangle = trianglesContainingVertex[i];
			for (var j = 0; j < 3; j++)
			{
				var vertexB = triangle[j];
				if (vertexB != vertexIndex && !_checkedVertices.Contains(vertexB))
				{
					if (IsOutlineEdge(vertexIndex, vertexB))
					{
						return vertexB;
					}
				}
			}
		}

		return -1;
	}

	private void CalculateMeshOutlines()
	{
		for (var vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
		{
			if (!_checkedVertices.Contains(vertexIndex))
			{
				var newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1)
				{
					_checkedVertices.Add(vertexIndex);
					_outlines.Add(new List<int>() { vertexIndex });
					FollowOutline(newOutlineVertex, _outlines.Count - 1);
					_outlines[_outlines.Count - 1].Add(vertexIndex);
				}
			}
		}
	}

	private void FollowOutline(int vertexIndex, int outlineIndex)
	{
		_outlines[outlineIndex].Add(vertexIndex);
		_checkedVertices.Add(vertexIndex);
		var nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
		if (nextVertexIndex != -1)
		{
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}
}