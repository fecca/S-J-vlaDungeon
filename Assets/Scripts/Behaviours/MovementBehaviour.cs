using UnityEngine;

public class MovementBehaviour : Behaviour, IMovement
{
	[SerializeField]
	private MovementData Data;

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
		if (_character.Agent.IsMoving)
		{
			return;
		}

		var random = Random.Range(0, 4);
		var direction = random == 0 ? -Vector3.right : random == 1 ? Vector3.forward : random == 2 ? Vector3.right : -Vector3.forward;
		_character.Agent.StartPathTo(transform.position + direction, Data.MovementSpeed);
	}
}