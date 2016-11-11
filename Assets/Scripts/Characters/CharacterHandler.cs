using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject PlayerPrefab;
	[SerializeField]
	private GameObject EnemyPrefab;
	[SerializeField]
	private int NumberOfEnemies;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<MapRegisteredEvent>(OnMapRegisteredEvent);
	}

	private void OnMapRegisteredEvent(MapRegisteredEvent mapGeneratedEvent)
	{
		SpawnPlayer();
		SpawnEnemies();

		MessageHub.Instance.Publish(new CharactersSpawnedEvent(null));
	}
	private void SpawnPlayer()
	{
		var player = Instantiate(PlayerPrefab) as GameObject;
		player.GetComponent<Character>().Setup(FindObjectOfType<PathFinder>());
	}
	private void SpawnEnemies()
	{
		for (var i = 0; i < 5; i++)
		{
			var enemy = Instantiate(EnemyPrefab) as GameObject;
			enemy.GetComponent<Character>().Setup(FindObjectOfType<PathFinder>());
		}
	}
}