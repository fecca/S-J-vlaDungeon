using UnityEngine;

public class WalkingThought : IThought
{
	private IMover _mover;
	private MoveData _data;
	private float _updateTimer;

	public WalkingThought(IMover mover, MoveData data)
	{
		_mover = mover;
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