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
		CreateEnemies();

		MessageHub.Instance.Publish(new CharactersSpawnedEvent(null));
	}
	private void OnEnemyDiedEvent(EnemyDiedEvent enemyDiedEvent)
	{
		CreateEnemy();
		CreateEnemy();
	}
	private void CreatePlayer()
	{
		var newGameObject = Instantiate(CharacterPrefab);
		newGameObject.layer = LayerMask.NameToLayer("Player");

		var player = CharacterFactory.CreatePlayer(newGameObject);
		player.Initialize(FindObjectOfType<PathFinder>());
	}
	private void CreateEnemies()
	{
		CreateEnemy();
	}
	private void CreateEnemy()
	{
		var newGameObject = Instantiate(CharacterPrefab);
		newGameObject.layer = LayerMask.NameToLayer("Enemy");
		newGameObject.GetComponentInChildren<Light>().gameObject.SetActive(false);

		var enemy = CharacterFactory.CreateEnemy(newGameObject, HealthType.Medium, AttackerType.Medium, MoverType.Medium, PerceptionType.Medium);
		enemy.Initialize(FindObjectOfType<PathFinder>());
	}
}