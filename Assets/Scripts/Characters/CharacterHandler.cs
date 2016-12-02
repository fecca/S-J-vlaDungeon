using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject CharacterPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<MapRegisteredEvent>(OnMapRegisteredEvent);
		MessageHub.Instance.Subscribe<EnemyDiedEvent>(OnEnemyDiedEvent);
	}

	private void OnMapRegisteredEvent(MapRegisteredEvent mapGeneratedEvent)
	{
		CreatePlayer();
		CreateEnemies(NumberOfEnemies);

		MessageHub.Instance.Publish(new CharactersSpawnedEvent(null));
	}
	private void CreatePlayer()
	{
		CharacterFactory.CreatePlayer(CharacterPrefab);
	}
	private void OnEnemyDiedEvent(EnemyDiedEvent enemyDiedEvent)
	{
		CreateEnemies(1);
	}
	private void CreateEnemies(int numberOfEnemies)
	{
		for (var i = 0; i < numberOfEnemies; i++)
		{
			CharacterFactory.CreateRandomEnemy(CharacterPrefab);
		}
	}
}