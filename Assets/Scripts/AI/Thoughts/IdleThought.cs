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
		Debug.Log("IdleThought.Enter");
		_brain.GetOwner().DeactivateLights();
	}
	public override void Think()
	{
	}
	public override void Exit()
	{
		Debug.Log("IdleThought.Exit");
		_brain.GetOwner().ActivateLights();
	}
}