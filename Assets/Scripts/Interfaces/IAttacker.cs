using UnityEngine;

public interface IAttacker
{
	void Attack(AttackData data, Vector3 direction);
}