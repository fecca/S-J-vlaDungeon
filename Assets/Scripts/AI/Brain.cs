using UnityEngine;

public class Brain
{
	private Enemy _owner;
	private Perception _perception;
	private Transform _target;
	private Thought _currentThought;
	private Thought _attackingThought;
	private Thought _idleThought;
	private Thought _walkingThought;
	private float _perceptionTimer;

	public ThoughtType CurrentThoughtType { get; private set; }

	public Brain(Enemy owner, Transform target)
	{
		_owner = owner;
		_target = target;
		_idleThought = new IdleThought(this);
		EnterThought(ThoughtType.Idle);
	}

	private void CheckPerception()
	{
		if (_perception == null)
		{
			return;
		}

		if (_perceptionTimer > _perception.GetRandomUpdateInterval())
		{
			_perceptionTimer = 0f;
			Perceive();
		}

		_perceptionTimer += Time.deltaTime;
	}
	private void Perceive()
	{
		var perceptionState = _perception.GetPerceptionState(_owner.transform, _target);
		switch (perceptionState)
		{
			case PerceptionState.Outside:
				EnterThought(ThoughtType.Idle);
				break;

			case PerceptionState.InnerCirle:
				EnterThought(ThoughtType.Attacking);
				break;

			case PerceptionState.OuterCircle:
				EnterThought(ThoughtType.Walking);
				break;

			case PerceptionState.BehindWall:
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

		if (_currentThought != null)
		{
			_currentThought.Think();
		}
	}
	public void EnterThought(ThoughtType thoughtType)
	{
		if (thoughtType == CurrentThoughtType)
		{
			return;
		}

		Thought nextThought;
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
		if (nextThought == null)
		{
			nextThought = _idleThought;
			CurrentThoughtType = ThoughtType.Idle;
		}

		if (_currentThought != null)
		{
			_currentThought.Exit();
		}
		_currentThought = nextThought;
		_currentThought.Enter();
	}
	public void SetToAttacker(AttackData attackData)
	{
		if (attackData != null)
		{
			_attackingThought = new AttackingThought(this, attackData);
		}
	}
	public void SetToMover(MoveData moveData)
	{
		if (moveData != null)
		{
			_walkingThought = new WalkingThought(this, moveData);
		}
	}
	public void SetToPerceiver(PerceptionData perceptionData)
	{
		if (perceptionData != null)
		{
			_perception = new Perception(perceptionData);
		}
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