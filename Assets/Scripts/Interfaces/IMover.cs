using UnityEngine;

public interface IMover : IBehaviour
{
	void Move(Vector3 position);
	void Stop();
}