using System;
using UnityEngine;

public interface IPathFinderAgent
{
	void Initialize();
	void StartPathTo(Vector3 targetPosition, float movementSpeed, Action completed = null);
	void SmoothStop();
	void ClearOccupiedNodes();
	void RotateAgent(Vector3 direction);
}