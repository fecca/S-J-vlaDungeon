using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static T GetRandomElement<T>(this IList<T> collection, int margin)
	{
		return collection[Random.Range(0 + margin, collection.Count - margin)];
	}
}