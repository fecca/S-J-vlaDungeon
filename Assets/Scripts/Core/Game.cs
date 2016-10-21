using UnityEngine;

public class Game : MonoBehaviour
{
	private MapGenerator _mapGenerator;
	private MeshGenerator _meshGenerator;
	private PathFinder _pathFinder;
	private PlayerController _player;

	private void Start()
	{
		_mapGenerator = FindObjectOfType<MapGenerator>();
		_meshGenerator = FindObjectOfType<MeshGenerator>();
		_pathFinder = FindObjectOfType<PathFinder>();
		_player = FindObjectOfType<PlayerController>();

		_mapGenerator.GenerateMap(_meshGenerator, _pathFinder);
		_player.SetPosition(_mapGenerator.GetPlayerPosition());
	}
}