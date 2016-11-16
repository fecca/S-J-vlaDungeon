using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject PlayerPrefab = null;
	[SerializeField]
	private GameObject EnemyPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<MapRegisteredEvent>(OnMapRegisteredEvent);
		MessageHub.Instance.Subscribe<EnemyDiedEvent>(OnEnemyDiedEvent);
	}

	private void OnMapRegisteredEvent(MapRegisteredEvent mapGeneratedEvent)
	{
		SpawnPlayer();
		SpawnEnemies();

		MessageHub.Instance.Publish(new CharactersSpawnedEvent(null));
	}
	private void OnEnemyDiedEvent(EnemyDiedEvent enemyDiedEvent)
	{
		SpawnEnemy();
		SpawnEnemy();
	}
	private void SpawnPlayer()
	{
		var player = Instantiate(PlayerPrefab) as GameObject;
		player.GetComponent<Character>().Setup(FindObjectOfType<PathFinder>());
	}
	private void SpawnEnemies()
	{
		for (var i = 0; i < NumberOfEnemies; i++)
		{
			SpawnEnemy();
		}
	}

	public void SpawnEnemy()
	{
		var enemy = Instantiate(EnemyPrefab) as GameObject;
		enemy.GetComponent<Character>().Setup(FindObjectOfType<PathFinder>());
	}
}