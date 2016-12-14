using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
	void GetPath(Vector3 from, Vector3 to, Action<LinkedList<PathfindingNode>> completed);
	PathfindingNode GetNodeCopy(Vector3 worldPosition);
	PathfindingNode GetRandomWalkableNode();
}