using UnityEngine;

public class WalkingThought : IThought
{
	private IMover _mover;
	private MoveData _data;
	private float _updateTimer;

	public WalkingThought(IMover brain, MoveData data)
	{
		_mover = brain;
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
			_mover.Move(_data, ServiceLocator<ICharacterHandler>.Instance.GetPlayer().GetTransformPosition());
		}

		_updateTimer += Time.deltaTime;
	}
	public void Exit()
	{
		_mover.SmoothStop();
	}
}