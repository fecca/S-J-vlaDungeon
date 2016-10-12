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
		MeshGenerator.GenerateMeshes(map);
		PlayerController.SetPosition(MapGenerator.GetPlayerPosition());
	}
}