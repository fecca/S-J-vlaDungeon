using UnityEngine;

public class Mover : MonoBehaviour, IMover
{
	[SerializeField]
	private MoverData Data;
	private Character _character;
	private Player _player;

	public void Initialize(Character character)
	{
		_character = character;
		_player = FindObjectOfType<Player>();
	}

	public void BehaviourUpdate()
	{
		//if (_character.Agent.IsMoving)
		//{
		//	return;
		//}

		Move();
	}

	public void Move()
	{
		_character.Agent.StartPathTo(_player.transform.position, Data.MovementSpeed, () =>
		{
			Debug.Log("Arrived...");
		});
	}
}