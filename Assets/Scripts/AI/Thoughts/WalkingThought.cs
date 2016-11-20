using UnityEngine;

public class WalkingThought : Thought
{
	private Brain _brain;
	private MoverData _data;
	private float _updateTimer;

	public WalkingThought(Brain brain)
	{
		_brain = brain;
		_data = ScriptableObject.CreateInstance<MoverData>();
	}

	public override void Enter()
	{
		Debug.Log("Enter " + GetType());
	}
	public override void Think()
	{
		if (_updateTimer > _data.PositionUpdateInterval)
		{
			_updateTimer = 0;
			_brain.GetOwner().Move(_data);
		}

		_updateTimer += Time.deltaTime;
	}
	public override void Exit()
	{
		Debug.Log("Exit " + GetType());
		_brain.GetOwner().Agent.SmoothStop();
	}
}