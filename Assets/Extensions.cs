using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static T GetRandomElement<T>(this IList<T> collection)
	{
		return collection[Random.Range(0, collection.Count)];
	}
}