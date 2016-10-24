using System;
using UnityEngine;

public class MovementBehaviour : Behaviour, IMovement
{
	protected override void RegisterBehaviour()
	{
		_character.RegisterBehaviour(this);
	}
	private void Update() { /* Keep empty */ }

	public void BehaviourUpdate()
	{
		Move();
	}
	public void Move()
	{
		_character.transform.Translate(Vector3.zero, Space.World);
	}
}

public abstract class Behaviour : MonoBehaviour
{
	protected Character _character;
	protected void Awake()
	{
		_character = GetComponent<Character>();
		if (_character == null)
		{
			throw new NotSupportedException("Can not add script to this object.");
		}

		RegisterBehaviour();
	}
	protected abstract void RegisterBehaviour();
}