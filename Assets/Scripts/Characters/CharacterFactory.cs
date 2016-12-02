using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CharacterFactory
{
	public static Player CreatePlayer(GameObject prefab)
	{
		var newGameObject = Object.Instantiate(prefab);
		newGameObject.name = "Player";
		newGameObject.layer = LayerMask.NameToLayer("Player");

		var player = newGameObject.AddComponent<Player>();
		player.InitializePathfindingAgent();

		var healthData = new HealthData(Constants.PLAYER_HEALTH);
		player.SetHealthData(healthData);

		var attackData = new AttackData(0, Constants.PLAYER_PROJECTILE_SPEED, "PlayerProjectile");
		player.SetAttackData(attackData);

		var moveData = new MoveData(0, Constants.PLAYER_MOVEMENT_SPEED);
		player.SetMoveData(moveData);

		return player;
	}
	public static Enemy CreateEnemy(
		GameObject prefab,
		HealthType healthType,
		AttackerType attackerType,
		MoverType moverType,
		PerceptionType perceptionType)
	{
		var newGameObject = Object.Instantiate(prefab);
		newGameObject.name = "Enemy";
		newGameObject.layer = LayerMask.NameToLayer("Enemy");
		newGameObject.GetComponentInChildren<Light>().gameObject.SetActive(false);

		var enemy = newGameObject.AddComponent<Enemy>();
		enemy.InitializeBrain();
		enemy.InitializePathfindingAgent();

		var healthData = GetHealthData(healthType);
		enemy.SetHealthData(healthData);

		var attackData = GetAttackerData(attackerType);
		enemy.SetAttackData(attackData);

		var moveData = GetMoverData(moverType);
		enemy.SetMoveData(moveData);

		var perceptionData = GetPerceptionData(perceptionType);
		enemy.SetPerceptionData(perceptionData);

		Debug.Log("Created Enemy: " +
			"Health: [" + healthType + "], " +
			"Attacker: [" + attackerType + "], " +
			"Mover: [" + moverType + "], " +
			"Perception: [" + perceptionType + "]");

		return enemy;
	}
	public static Enemy CreateRandomEnemy(
		GameObject prefab)
	{
		return CreateEnemy(
			prefab,
			GetRandomOfType<HealthType>(),
			GetRandomOfType<AttackerType>(),
			GetRandomOfType<MoverType>(),
			GetRandomOfType<PerceptionType>());
	}

	private static T GetRandomOfType<T>()
	{
		var values = Enum.GetValues(typeof(T));
		var random = UnityEngine.Random.Range(0, values.Length);
		var value = values.GetValue(random);

		return (T)value;
	}
	private static HealthData GetHealthData(HealthType healthType)
	{
		switch (healthType)
		{
			case HealthType.Low:
				return new HealthData(Constants.LOW_HEALTH);
			case HealthType.Medium:
				return new HealthData(Constants.MEDIUM_HEALTH);
			case HealthType.High:
				return new HealthData(Constants.HIGH_HEALTH);
			default:
				throw new NotImplementedException("HealthType not implemented: " + healthType);
		}
	}
	private static AttackData GetAttackerData(AttackerType attackerType)
	{
		switch (attackerType)
		{
			case AttackerType.None:
				return null;
			case AttackerType.Slow:
				return new AttackData(Constants.SLOW_TIME_BETWEEN_ATTACKS, Constants.SLOW_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			case AttackerType.Medium:
				return new AttackData(Constants.MEDIUM_TIME_BETWEEN_ATTACKS, Constants.MEDIUM_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			case AttackerType.Fast:
				return new AttackData(Constants.FAST_TIME_BETWEEN_ATTACKS, Constants.FAST_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			default:
				throw new NotImplementedException("AttackerType not implemented: " + attackerType);
		}
	}
	private static MoveData GetMoverData(MoverType moverType)
	{
		switch (moverType)
		{
			case MoverType.None:
				return null;
			case MoverType.Slow:
				return new MoveData(Constants.SLOW_POSITION_UPDATE_INTERVAL, Constants.SLOW_MOVEMENT_SPEED);
			case MoverType.Medium:
				return new MoveData(Constants.MEDIUM_POSITION_UPDATE_INTERVAL, Constants.MEDIUM_MOVEMENT_SPEED);
			case MoverType.Fast:
				return new MoveData(Constants.FAST_POSITION_UPDATE_INTERVAL, Constants.FAST_MOVEMENT_SPEED);
			default:
				throw new NotImplementedException("MoverType not implemented: " + moverType);
		}
	}
	private static PerceptionData GetPerceptionData(PerceptionType perceptionType)
	{
		switch (perceptionType)
		{
			case PerceptionType.Low:
				return new PerceptionData(Constants.LOW_PERCEPTION_UPDATE_INTERVAL, Constants.LOW_INNER_RADIUS, Constants.LOW_OUTER_RADIUS);
			case PerceptionType.Medium:
				return new PerceptionData(Constants.MEDIUM_PERCEPTION_UPDATE_INTERVAL, Constants.MEDIUM_INNER_RADIUS, Constants.MEDIUM_OUTER_RADIUS);
			case PerceptionType.High:
				return new PerceptionData(Constants.HIGH_PERCEPTION_UPDATE_INTERVAL, Constants.HIGH_INNER_RADIUS, Constants.HIGH_OUTER_RADIUS);
			default:
				throw new NotImplementedException("PerceptionType not implemented: " + perceptionType);
		}
	}
}