using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static T GetRandomElement<T>(this IList<T> collection, int margin = 0)
	{
		return collection[Random.Range(0 + margin, collection.Count - margin)];
	}
	public static PathfindingNode Copy(this PathfindingNode node)
	{
		return new PathfindingNode(node.X, node.Y, node.Walkable);
	}
}