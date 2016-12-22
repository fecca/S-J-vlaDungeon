using System;
using UnityEngine;

public class CharacterFactory
{
	public Player CreatePlayer(GameObject prefab)
	{
		var newGameObject = UnityEngine.Object.Instantiate(prefab);
		newGameObject.name = "Player";
		newGameObject.layer = LayerMask.NameToLayer("Player");

		var player = newGameObject.AddComponent<Player>();
		player.InitializeBrain();
		player.InitializePathfindingAgent();
		player.InitializeHealth(new HealthData(Constants.PLAYER_HEALTH));
		player.InitializeAttacker(new AttackData(0, Constants.PLAYER_PROJECTILE_SPEED, "PlayerProjectile"));
		player.InitializeMover(new MoveData(0, Constants.PLAYER_MOVEMENT_SPEED));
		player.InitializeInventory();

		return player;
	}
	public Enemy CreateEnemy(
		GameObject prefab,
		HealthType healthType,
		AttackerType attackerType,
		MoverType moverType,
		PerceptionType perceptionType,
		ICharacter target)
	{
		var newGameObject = UnityEngine.Object.Instantiate(prefab);
		newGameObject.name = "Enemy";
		newGameObject.layer = LayerMask.NameToLayer("Enemy");

		var enemy = newGameObject.AddComponent<Enemy>();
		enemy.InitializeBrain();
		enemy.InitializePathfindingAgent();
		enemy.InitializeHealth(GetHealthData(healthType));
		enemy.InitializeAttacker(GetAttackerData(attackerType));
		enemy.InitializeMover(GetMoverData(moverType));
		enemy.InitializeLoot();
		enemy.InitializePerception(GetPerceptionData(perceptionType));
		enemy.InitializeTarget(target);

		return enemy;
	}
	public Enemy CreateRandomEnemy(
		GameObject prefab,
		ICharacter target)
	{
		return CreateEnemy(
			prefab,
			Util.GetRandomEnumValue<HealthType>(),
			Util.GetRandomEnumValue<AttackerType>(),
			Util.GetRandomEnumValue<MoverType>(),
			Util.GetRandomEnumValue<PerceptionType>(),
			target);
	}

	private HealthData GetHealthData(HealthType healthType)
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
	private AttackData GetAttackerData(AttackerType attackerType)
	{
		switch (attackerType)
		{
			case AttackerType.None:
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
	private MoveData GetMoverData(MoverType moverType)
	{
		switch (moverType)
		{
			case MoverType.None:
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
	private PerceptionData GetPerceptionData(PerceptionType perceptionType)
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