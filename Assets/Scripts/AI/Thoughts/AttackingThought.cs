using UnityEngine;

public class AttackingThought : IThought
{
	private Brain _brain;
	private AttackData _data;
	private float _updateTimer;

	public AttackingThought(Brain brain, AttackData data)
	{
		_brain = brain;
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
			_brain.Attack(_data);
		}

		_updateTimer += Time.deltaTime;
	}
	public void Exit()
	{
	}
}