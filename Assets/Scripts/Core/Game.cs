using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField]
	private MapGenerator MapGenerator = null;
	[SerializeField]
	private MeshGenerator MeshGenerator = null;
	[SerializeField]
	private PlayerController Player = null;

	private void Start()
	{
		var map = MapGenerator.GenerateMap();
		FindObjectOfType<PathFinder>().RegisterMap(map);
		MeshGenerator.GenerateMeshes(map);
		Player.SetPosition(MapGenerator.GetPlayerPosition());

		var enemy = FindObjectOfType<Enemy>();
		enemy.SetPosition(MapGenerator.GetPlayerPosition());
	}
}