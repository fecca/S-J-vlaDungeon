using UnityEngine;

public class AttackingThought : IThought
{
	private IAttacker _attacker;
	private AttackData _data;
	private float _updateTimer;

	public AttackingThought(IAttacker attacker, AttackData data)
	{
		_attacker = attacker;
		_data = data;
	}

	public void Enter()
	{
	}
	public void Think()
	{
		if (_updateTimer > _data.TimeBetweenAttacks)
		{
			_updateTimer = 0;
			_attacker.Attack(_data, Vector3.zero);
		}

		_updateTimer += Time.deltaTime;
	}
	public void Exit()
	{
	}
}