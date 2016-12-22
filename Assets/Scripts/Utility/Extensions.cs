using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static T GetRandomElement<T>(this IList<T> collection, int margin = 0)
	{
		return collection[Random.Range(0 + margin, collection.Count - margin)];
	}
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		var component = gameObject.GetComponent<T>();
		if (component == null)
		{
			component = gameObject.AddComponent<T>();
		}

		return component;
	}
	public static Vector3 WithX(this Vector3 vector, float x)
	{
		return new Vector3(x, vector.y, vector.z);
	}
	public static Vector3 WithY(this Vector3 vector, float y)
	{
		return new Vector3(vector.x, y, vector.z);
	}
	public static Vector3 WithZ(this Vector3 vector, float z)
	{
		return new Vector3(vector.x, vector.y, z);
	}
	public static Vector3 GetDirectionTo(this Transform from, Transform to)
	{
		return (to.position - from.position).normalized;
	}
	public static Vector3 GetDirectionTo(this Transform from, Vector3 to)
	{
		return (to - from.position).normalized;
	}
	public static Vector3 GetDirectionTo(this Vector3 from, Transform to)
	{
		return (to.position - from).normalized;
	}
	public static Vector3 GetDirectionTo(this Vector3 from, Vector3 to)
	{
		return (to - from).normalized;
	}
	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count <= 0;
	}
}