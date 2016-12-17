using UnityEngine;

public interface ICharacter
{
	HealthData HealthData { get; set; }
	GameObject GetGameObject();
	Vector3 GetTransformPosition();
	void InitializeBrain();
	void InitializePathfindingAgent();
	void InitializeAttacker(AttackData attackData);
	void InitializeMover(MoveData moveData);
	void InitializeHealth(HealthData healthData);
	void TakeDamage();
}