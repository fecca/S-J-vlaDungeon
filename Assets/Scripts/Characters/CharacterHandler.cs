using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject CharacterPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private Player _player;
	private Enemy[] _enemies;

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
		Destroy(_player.gameObject);
		for (var i = 0; i < _enemies.Length; i++)
		{
			Destroy(_enemies[i].gameObject);
		}

		MessageHub.Instance.Publish(new CharactersDestroyedEvent(null));
	}
	private void CreatePlayer()
	{
		_player = CharacterFactory.CreatePlayer(CharacterPrefab);
		ServiceLocator<ICharacter>.Instance = _player;
	}
	private void CreateEnemies(int numberOfEnemies)
	{
		_enemies = new Enemy[numberOfEnemies];
		for (var i = 0; i < numberOfEnemies; i++)
		{
			_enemies[i] = CharacterFactory.CreateRandomEnemy(CharacterPrefab);
		}
	}
}