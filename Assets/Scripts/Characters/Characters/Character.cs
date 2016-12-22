using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
	public HealthData HealthData { get; set; }

	public abstract GameObject GetGameObject();
	public abstract Vector3 GetTransformPosition();
	public abstract void InitializeAttacker(AttackData attackData);
	public abstract void InitializeBrain();
	public abstract void InitializeHealth(HealthData healthData);
	public abstract void InitializeMover(MoveData moveData);
	public abstract void InitializePathfindingAgent();
	public abstract void TakeDamage();
}