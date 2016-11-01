using UnityEngine;

public class Attacker : MonoBehaviour, IAttacker
{
	[SerializeField]
	private AttackerData Data;
	private Character _character;

	public void Initialize(Character character)
	{
		_character = character;
	}

	public void BehaviourUpdate()
	{
	}

	public void Attack()
	{
	}
}