using UnityEngine;

public class CharacterHandler : MonoBehaviour, ICharacterHandler
{
	[SerializeField]
	private GameObject CharacterPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private CharacterFactory _characterFactory;
	private ICharacter _player;
	private ICharacter[] _enemies;

	private void Awake()
	{
		ServiceLocator<ICharacterHandler>.Instance = this;
		_characterFactory = new CharacterFactory();
		MessageHub.Instance.Subscribe<PathNodesCreatedEvent>(OnPathNodesCreatedEvent);
		MessageHub.Instance.Subscribe<DestroyGameEvent>(OnDestroyGameEvent);
	}

	public ICharacter GetPlayer()
	{
		return _player;
	}

	private void OnPathNodesCreatedEvent(PathNodesCreatedEvent pathNodesCreatedEvent)
	{
		CreatePlayer();
		CreateEnemies(NumberOfEnemies);

		MessageHub.Instance.Publish(new CharactersCreatedEvent(null));
	}
	private void OnDestroyGameEvent(DestroyGameEvent destroyGameEvent)
	{
		Destroy(_player.GetGameObject());
		for (var i = 0; i < _enemies.Length; i++)
		{
			Destroy(_enemies[i].GetGameObject());
		}

		MessageHub.Instance.Publish(new CharactersDestroyedEvent(null));
	}
	private void CreatePlayer()
	{
		_player = _characterFactory.CreatePlayer(CharacterPrefab);
	}
	private void CreateEnemies(int numberOfEnemies)
	{
		_enemies = new Enemy[numberOfEnemies];
		for (var i = 0; i < numberOfEnemies; i++)
		{
			_enemies[i] = _characterFactory.CreateRandomEnemy(CharacterPrefab, _player);
		}
	}
}