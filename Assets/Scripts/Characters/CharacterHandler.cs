using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject CharacterPrefab = null;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<MapRegisteredEvent>(OnMapRegisteredEvent);
		MessageHub.Instance.Subscribe<EnemyDiedEvent>(OnEnemyDiedEvent);
	}

	private void OnMapRegisteredEvent(MapRegisteredEvent mapGeneratedEvent)
	{
		CreatePlayer();
		//CreateEnemies();

		MessageHub.Instance.Publish(new CharactersSpawnedEvent(null));
	}
	private void CreatePlayer()
	{
		CharacterFactory.CreatePlayer(CharacterPrefab);
	}
	private void CreateEnemy()
	{
		CharacterFactory.CreateRandomEnemy(CharacterPrefab);
	}
	private void OnEnemyDiedEvent(EnemyDiedEvent enemyDiedEvent)
	{
		CreateEnemy();
	}
	private void CreateEnemies()
	{
		for (var i = 0; i < 10; i++)
		{
			CreateEnemy();
		}
	}
}