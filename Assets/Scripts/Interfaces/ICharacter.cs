using UnityEngine;

public interface ICharacter
{
	HealthData HealthData { get; set; }
	IPathFinderAgent Agent { get; }
	GameObject GetGameObject();
	Vector3 GetTransformPosition();
	void InitializePathfindingAgent();
	void InitializeAttacker(AttackData attackData);
	void InitializeMover(MoveData moveData);
	void InitializerPerception(PerceptionData perceptionData);
	void InitializeHealth(HealthData healthData);
	void TakeDamage();
}