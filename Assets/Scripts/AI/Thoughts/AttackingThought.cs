using UnityEngine;

public class AttackingThought : Thought
{
	private Brain _brain;
	private AttackerData _data;
	private float _updateTimer;

	public AttackingThought(Brain brain)
	{
		_brain = brain;
		_data = ScriptableObject.CreateInstance<AttackerData>();
		_data.ProjectilePrefab = Resources.Load<GameObject>("EnemyProjectile");
	}

	public override void Enter()
	{
	}
	public override void Think()
	{
		if (_updateTimer > _data.TimeBetweenAttacks)
		{
			_updateTimer = 0;
			_brain.GetOwner().Attack(_data);
		}

		_updateTimer += Time.deltaTime;
	}
	public override void Exit()
	{
	}
}