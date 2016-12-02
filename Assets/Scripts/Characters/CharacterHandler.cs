using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject CharacterPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<PathNodesCreatedEvent>(OnPathNodesCreatedEvent);
		MessageHub.Instance.Subscribe<DestroyGameEvent>(OnDestroyGameEvent);
	}

	private void OnPathNodesCreatedEvent(PathNodesCreatedEvent pathNodesCreatedEvent)
	{
		CreatePlayer();
		CreateEnemies(NumberOfEnemies);

		MessageHub.Instance.Publish(new CharactersCreatedEvent(null));
	}
	private void OnDestroyGameEvent(DestroyGameEvent destroyGameEvent)
	{
		Destroy(FindObjectOfType<Player>().gameObject);
		var enemies = FindObjectsOfType<Enemy>();
		for (var i = 0; i < enemies.Length; i++)
		{
			Destroy(enemies[i].gameObject);
		}

		MessageHub.Instance.Publish(new CharactersDestroyedEvent(null));
	}
	private void CreatePlayer()
	{
		CharacterFactory.CreatePlayer(CharacterPrefab);
	}
	private void CreateEnemies(int numberOfEnemies)
	{
		for (var i = 0; i < numberOfEnemies; i++)
		{
			CharacterFactory.CreateRandomEnemy(CharacterPrefab);
		}
	}
}