using UnityEngine;

public class CharacterHandler : MonoBehaviour, ICharacterHandler
{
	[SerializeField]
	private GameObject PlayerPrefab = null;
	[SerializeField]
	private GameObject EnemyPrefab = null;
	[SerializeField]
	private int NumberOfEnemies = 1;

	private CharacterFactory _characterFactory;
	private Player _player;
	private ICharacter[] _enemies;

	private void Awake()
	{
		ServiceLocator<ICharacterHandler>.Instance = this;

		_characterFactory = new CharacterFactory();

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
		Destroy(_player.GetGameObject());
		for (var i = 0; i < _enemies.Length; i++)
		{
			Destroy(_enemies[i].GetGameObject());
		}

		MessageHub.Instance.Publish(new CharactersDestroyedEvent(null));
	}

	public Player GetPlayer()
	{
		return _player;
	}

	private void CreatePlayer()
	{
		_player = _characterFactory.CreatePlayer(PlayerPrefab);
	}
	private void CreateEnemies(int numberOfEnemies)
	{
		_enemies = new Enemy[numberOfEnemies];
		for (var i = 0; i < numberOfEnemies; i++)
		{
			_enemies[i] = _characterFactory.CreateEnemy(EnemyPrefab, HealthType.Low, AttackerType.Medium, MoverType.Medium, PerceptionType.Medium, _player);
		}
	}
}