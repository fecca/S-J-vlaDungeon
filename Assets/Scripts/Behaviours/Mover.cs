using System;
using UnityEngine;

public class Mover : MonoBehaviour, IMover
{
	[SerializeField]
	private MoverData Data;

	private Character _character;

	public void BehaviourUpdate()
	{
		throw new NotImplementedException();
	}

	public void Initialize(Character character)
	{
		_character = character;
	}

	public void Move(Vector3 position)
	{
		_character.Agent.StartPathTo(position, Data.MovementSpeed, () =>
		{
			Debug.Log("Arrived...");
		});
	}

	public void Stop()
	{
		_character.Agent.Stop();
	}
}