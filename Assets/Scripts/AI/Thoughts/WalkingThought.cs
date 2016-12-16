using UnityEngine;

public class WalkingThought : IThought
{
	private EnemyBrain _brain;
	private MoveData _data;
	private float _updateTimer;

	public WalkingThought(EnemyBrain brain, MoveData data)
	{
		_brain = brain;
		_data = data;
	}

	public void Enter()
	{
	}
	public void Think()
	{
		if (_updateTimer > _data.PositionUpdateInterval)
		{
			_updateTimer = 0;
			_brain.Move(_data, ServiceLocator<ICharacterHandler>.Instance.GetPlayer().GetTransformPosition());
		}

		_updateTimer += Time.deltaTime;
	}
	public void Exit()
	{
		_brain.SmoothStop();
	}
}