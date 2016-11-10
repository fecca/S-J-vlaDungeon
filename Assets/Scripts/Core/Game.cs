using System;
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

		SpawnPlayer();
		SpawnEnemies();
	}

	private void SpawnPlayer()
	{
		_player.Setup(_pathFinder);
	}

	private void SpawnEnemies()
	{
		for (var i = 0; i < NumberOfEnemies; i++)
		{
			var randomTile = _mapGenerator.GetRandomWalkableTile();
			var randomTilePosition = new Vector3(randomTile.WorldCoordinates.x, 0.5f, randomTile.WorldCoordinates.z);
			var enemyGameObject = Instantiate(EnemyPrefab, randomTilePosition, Quaternion.identity) as GameObject;
			enemyGameObject.GetComponent<Character>().Setup(_pathFinder);
		}
	}
}