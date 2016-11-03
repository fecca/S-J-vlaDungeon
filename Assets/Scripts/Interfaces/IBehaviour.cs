using UnityEngine;

public interface IBehaviour
{
	void Stop();
	void Start();
	void UpdateBehaviour(Transform targetTransform);
}