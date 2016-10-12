using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField]
	private MapGenerator MapGenerator = null;
	[SerializeField]
	private MeshGenerator MeshGenerator = null;
	[SerializeField]
	private PlayerController Controller = null;

	private void Start()
	{
		var map = MapGenerator.GenerateMap();
		MeshGenerator.GenerateMeshes(map);
		Controller.SetPosition(MapGenerator.GetPlayerPosition());
	}
}