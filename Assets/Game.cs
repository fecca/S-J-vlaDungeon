using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField]
	private MapGenerator MapGenerator = null;
	[SerializeField]
	private MeshGenerator MeshGenerator = null;
	[SerializeField]
	private PlayerController PlayerController = null;

	private void Start()
	{
		var map = MapGenerator.GenerateMap();
		MeshGenerator.GenerateMeshes(map, Constants.TileSize);
		PlayerController.SetPosition(MapGenerator.GetPlayerPosition());
	}
}

public class Constants
{
	public const int TileSize = 5;
}

public static class Extensions
{
	public static T GetRandomElement<T>(this IList<T> collection)
	{
		return collection[Random.Range(0, collection.Count)];
	}
}