using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField]
	private GameObject EnemyPrefab;
	[SerializeField]
	private int NumberOfEnemies;

	private MapGenerator _mapGenerator;
	private MeshGenerator _meshGenerator;
	private PathFinder _pathFinder;
	private Player _player;

	private void Start()
	{
		_mapGenerator = FindObjectOfType<MapGenerator>();
		_meshGenerator = FindObjectOfType<MeshGenerator>();
		_pathFinder = FindObjectOfType<PathFinder>();
		_player = FindObjectOfType<Player>();

		_mapGenerator.GenerateMap(_meshGenerator, _pathFinder);
		_player.transform.position = _mapGenerator.GetPlayerPosition() + Vector3.up;

		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		for (var i = 0; i < NumberOfEnemies; i++)
		{
			var randomTile = _mapGenerator.GetRandomWalkableTile();
			var randomTilePosition = new Vector3(randomTile.WorldCoordinates.X, 0.5f, randomTile.WorldCoordinates.Y);
			Instantiate(EnemyPrefab, randomTilePosition, Quaternion.identity);
		}
	}
}