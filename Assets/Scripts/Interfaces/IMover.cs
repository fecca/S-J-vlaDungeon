using UnityEngine;

public interface IMover
{
	void Move(MoveData data, Vector3 targetPosition);
	void SmoothStop();
}