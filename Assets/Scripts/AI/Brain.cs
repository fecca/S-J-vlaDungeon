using UnityEngine;

public class Brain
{
	private ICharacter _owner;
	private ICharacter _target;
	private IMover _mover;
	private IAttacker _attacker;
	private Perception _perception;
	private IThought _currentThought;
	private IThought _attackingThought;
	private IThought _idleThought;
	private IThought _walkingThought;
	private float _perceptionTimer;

	public ThoughtType CurrentThoughtType { get; private set; }

	public Brain(ICharacter owner)
	{
		_owner = owner;
		_idleThought = new IdleThought();
		EnterThought(ThoughtType.Idle);
	}

	public void InitializeAttacker(AttackData attackData, IAttacker attacker)
	{
		if (attackData != null)
		{
			_attacker = attacker;
			_attackingThought = new AttackingThought(this, attackData);
		}
	}
	public void InitializeMover(MoveData moveData, IMover mover)
	{
		if (moveData != null)
		{
			_mover = mover;
			_walkingThought = new WalkingThought(this, moveData);
		}
	}
	public void InitializePerception(PerceptionData perceptionData)
	{
		if (perceptionData != null)
		{
			_perception = new Perception(perceptionData);
		}
	}

	public void EnterThought(ThoughtType thoughtType)
	{
		if (thoughtType == CurrentThoughtType)
		{
			return;
		}

		IThought nextThought;
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
	public void SetTarget(ICharacter target)
	{
		_target = target;
	}
	public void Think()
	{
		if (_currentThought != null)
		{
			_currentThought.Think();
		}
	}
	public void Move(MoveData data, Vector3 targetPosition)
	{
		_mover.Move(data, targetPosition);
	}
	public void SmoothStop()
	{
		_mover.SmoothStop();
	}
	public void Attack(AttackData data)
	{
		var targetPosition = _target.GetTransformPosition().WithY(_owner.GetTransformPosition().y);
		var direction = _owner.GetTransformPosition().GetDirectionTo(targetPosition);
		_attacker.Attack(data, direction);
	}

	public void Perceive()
	{
		if (_perception == null)
		{
			return;
		}

		if (_perceptionTimer > _perception.GetRandomUpdateInterval())
		{
			_perceptionTimer = 0f;
			HandlePerception();
		}

		_perceptionTimer += Time.deltaTime;
	}
	public void HandlePerception()
	{
		var perceptionState = _perception.GetPerceptionState(_owner.GetTransformPosition(), _target.GetTransformPosition());
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
}