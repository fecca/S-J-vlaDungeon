using UnityEngine;

public class WalkingThought : Thought
{
	private Brain _brain;
	private MoveData _data;
	private float _updateTimer;

	public WalkingThought(Brain brain, MoveData data)
	{
		_brain = brain;
		_data = data;
	}

	public override void Enter()
	{
	}
	public override void Think()
	{
		if (_updateTimer > _data.PositionUpdateInterval)
		{
			_updateTimer = 0;
			_brain.Move(_data, ServiceLocator<ICharacterHandler>.Instance.GetPlayer().GetTransformPosition());
		}

		_updateTimer += Time.deltaTime;
	}
	public override void Exit()
	{
		_brain.SmoothStop();
	}
}