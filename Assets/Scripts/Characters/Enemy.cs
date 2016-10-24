using System.Collections.Generic;

public class Enemy : Character
{
	private void Update()
	{
		foreach (var behaviour in _behaviours)
		{
			behaviour.BehaviourUpdate();
		}
	}
}