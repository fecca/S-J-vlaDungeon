using UnityEngine;

public class Brain
{
	private Enemy _owner;
	private Perception _perception;
	private Transform _target;
	private Thought _currentThought;
	private Thought _idleThought;
	private Thought _walkingThought;
	private Thought _attackingThought;
	private float _perceptionTimer;

	public ThoughtType CurrentThoughtType { get; private set; }

	public Brain(Enemy owner, Transform target)
	{
		_owner = owner;
		_perception = new Perception();
		_target = target;
		_idleThought = new IdleThought(this);
		_walkingThought = new WalkingThought(this);
		_attackingThought = new AttackingThought(this);
	}

	private void CheckPerception()
	{
		if (_perceptionTimer > _perception.GetRandomUpdateInterval())
		{
			_perceptionTimer = 0f;
			Perceive();
		}

		_perceptionTimer += Time.deltaTime;
	}
	private void Perceive()
	{
		var perceptionState = _perception.GetPerceptionState(_owner.transform.position, _target.position);
		switch (perceptionState)
		{
			case PlayerPosition.Outside:
				EnterThought(ThoughtType.Idle);
				break;

			case PlayerPosition.InnerCirle:
				EnterThought(ThoughtType.Attacking);
				break;

			case PlayerPosition.OuterCircle:
				EnterThought(ThoughtType.Walking);
				break;

			case PlayerPosition.BehindWall:
				if (CurrentThoughtType == ThoughtType.Walking || CurrentThoughtType == ThoughtType.Attacking)
				{
					EnterThought(ThoughtType.Walking);
				}
				else
				{
					EnterThought(ThoughtType.Idle);
				}
				break;

			default:
				throw new System.NotImplementedException("PlayerPosition type not implemented: " + perceptionState);
		}
	}

	public void Think()
	{
		CheckPerception();
		if (_currentThought == null)
		{
			return;
		}
		_currentThought.Think();
	}
	public void EnterThought(ThoughtType thoughtType)
	{
		if (thoughtType == CurrentThoughtType)
		{
			return;
		}

		Thought nextThought = null;
		switch (thoughtType)
		{
			case ThoughtType.Idle:
				nextThought = _idleThought;
				break;
			case ThoughtType.Walking:
				nextThought = _walkingThought;
				break;
			case ThoughtType.Attacking:
				nextThought = _attackingThought;
				break;
			default:
				throw new System.NotImplementedException("ThoughtType not implemented: " + thoughtType);
		}
		CurrentThoughtType = thoughtType;

		if (_currentThought != null)
		{
			_currentThought.Exit();
		}
		_currentThought = nextThought;
		_currentThought.Enter();
	}
	public Enemy GetOwner()
	{
		return _owner;
	}
	public Vector3 GetTargetPosition()
	{
		return _target.position;
	}
}