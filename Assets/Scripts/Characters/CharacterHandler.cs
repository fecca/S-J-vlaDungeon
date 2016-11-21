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
		var playerObject = Instantiate(PlayerPrefab) as GameObject;
		var player = playerObject.GetComponent<Player>();

		player.Initialize(FindObjectOfType<PathFinder>());

		var healthData = ScriptableObject.CreateInstance<HealthData>();
		healthData.Health = 100;
		player.SetHealth(healthData);

		var attackerData = ScriptableObject.CreateInstance<AttackerData>();
		attackerData.ProjectilePrefab = Resources.Load<GameObject>("PlayerProjectile");
		player.SetToAttacker(attackerData);

		var moverData = ScriptableObject.CreateInstance<MoverData>();
		player.SetToMover(moverData);
	}
	private void CreateEnemies()
	{
		for (var i = 0; i < NumberOfEnemies; i++)
		{
			CreateEnemy();
		}
	}

	public void CreateEnemy()
	{
		var enemyObject = Instantiate(EnemyPrefab) as GameObject;
		var enemy = enemyObject.GetComponent<Enemy>();

		enemy.Initialize(FindObjectOfType<PathFinder>());

		var healthData = ScriptableObject.CreateInstance<HealthData>();
		healthData.Health = 4;
		enemy.SetHealth(healthData);

		var attackerData = ScriptableObject.CreateInstance<AttackerData>();
		attackerData.ProjectilePrefab = Resources.Load<GameObject>("EnemyProjectile");
		enemy.SetToAttacker(attackerData);

		var moverData = ScriptableObject.CreateInstance<MoverData>();
		enemy.SetToMover(moverData);
	}
}