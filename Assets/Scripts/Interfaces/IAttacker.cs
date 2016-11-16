using UnityEngine;

public interface IAttacker : IBehaviour
{
	void Attack(Vector3 targetPosition);
}