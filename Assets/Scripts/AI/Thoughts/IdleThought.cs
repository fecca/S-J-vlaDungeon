using UnityEngine;

public class IdleThought : Thought
{
	private Brain _brain;

	public IdleThought(Brain brain)
	{
		_brain = brain;
	}

	public override void Enter()
	{
		Debug.Log("Enter " + GetType());
	}
	public override void Think()
	{
	}
	public override void Exit()
	{
		Debug.Log("Exit " + GetType());
	}
}