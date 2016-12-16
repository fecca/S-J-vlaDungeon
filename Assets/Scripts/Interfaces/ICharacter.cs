using UnityEngine;

public interface ICharacter
{
	HealthData HealthData { get; set; }
	PathFinderAgent Agent { get; }
	GameObject GetGameObject();
	Vector3 GetTransformPosition();
	void InitializeBrain();
	void InitializePathfindingAgent();
	void InitializeAttacker(AttackData attackData);
	void InitializeMover(MoveData moveData);
	void InitializeHealth(HealthData healthData);
	void TakeDamage();
}