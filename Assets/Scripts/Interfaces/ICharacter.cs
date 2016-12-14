using UnityEngine;

public interface ICharacter
{
	PathFinderAgent Agent { get; }
	void InitializePathfindingAgent();
	void SetHealthData(HealthData healthData);
	void SetAttackData(AttackData attackData);
	void SetMoveData(MoveData moveData);
	void SetPerceptionData(PerceptionData perceptionData);
	void TakeDamage();
	Vector3 GetTransformPosition();
	void Attack(AttackData data, Vector3 direction);
	void Move(MoveData data, Vector3 targetPosition);
}