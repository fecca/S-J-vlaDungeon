using UnityEngine;

public static class CharacterFactory
{
	//public static Character CreateCharacter<T>(
	//	GameObject gameObject,
	//	HealthType healthType,
	//	AttackerType attackerType,
	//	MoverType moverType,
	//	PerceptionType perceptionType) where T : Character
	//{
	//	if (typeof(T).Equals(typeof(Player)))
	//	{
	//		return CreatePlayer(gameObject);
	//	}

	//	if (typeof(T).Equals(typeof(Enemy)))
	//	{
	//		return CreateEnemy(gameObject, healthType, attackerType, moverType, perceptionType);
	//	}

	//	return null;
	//}

	public static Player CreatePlayer(GameObject gameObject)
	{
		var player = gameObject.AddComponent<Player>();
		player.SetData(
			new HealthData(Constants.PLAYER_HEALTH),
			new AttackData(0, Constants.PLAYER_PROJECTILE_SPEED, "PlayerProjectile"),
			new MoveData(0, Constants.PLAYER_MOVEMENT_SPEED),
			null);

		return player;
	}
	public static Enemy CreateEnemy(
		GameObject gameObject,
		HealthType healthType,
		AttackerType attackerType,
		MoverType moverType,
		PerceptionType perceptionType)
	{
		var enemy = gameObject.AddComponent<Enemy>();
		enemy.SetData(
			GetHealthData(healthType),
			GetAttackerData(attackerType),
			GetMoverData(moverType),
			GetPerceptionData(perceptionType));

		return enemy;
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
				throw new System.NotImplementedException("HealthType not implemented: " + healthType);
		}
	}
	private static AttackData GetAttackerData(AttackerType attackerType)
	{
		switch (attackerType)
		{
			case AttackerType.Slow:
				return new AttackData(Constants.SLOW_TIME_BETWEEN_ATTACKS, Constants.SLOW_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			case AttackerType.Medium:
				return new AttackData(Constants.MEDIUM_TIME_BETWEEN_ATTACKS, Constants.MEDIUM_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			case AttackerType.Fast:
				return new AttackData(Constants.FAST_TIME_BETWEEN_ATTACKS, Constants.FAST_PROJECTILE_SPEED, Constants.ENEMY_PROJECTILE_NAME);
			default:
				throw new System.NotImplementedException("AttackerType not implemented: " + attackerType);
		}
	}
	private static MoveData GetMoverData(MoverType moverType)
	{
		switch (moverType)
		{
			case MoverType.Slow:
				return new MoveData(Constants.SLOW_POSITION_UPDATE_INTERVAL, Constants.SLOW_MOVEMENT_SPEED);
			case MoverType.Medium:
				return new MoveData(Constants.MEDIUM_POSITION_UPDATE_INTERVAL, Constants.MEDIUM_MOVEMENT_SPEED);
			case MoverType.Fast:
				return new MoveData(Constants.FAST_POSITION_UPDATE_INTERVAL, Constants.FAST_MOVEMENT_SPEED);
			default:
				throw new System.NotImplementedException("MoverType not implemented: " + moverType);
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
				throw new System.NotImplementedException("PerceptionType not implemented: " + perceptionType);
		}
	}
}