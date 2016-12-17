using System;
using UnityEngine;

public class EnemyBrain : IBrain, IAttacker, IMover, IPerceiver
{
	private ICharacter _owner;
	private ICharacter _target;
	private IPathFinderAgent _agent;
	private IThought _currentThought;
	private IThought _attackingThought;
	private IThought _idleThought;
	private IThought _walkingThought;
	private Perception _perception;
	private float _perceptionTimer;

	public IPathFinderAgent Agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = _owner.GetGameObject().GetComponent<PathFinderAgent>();
			}
			return _agent;
		}
	}
	public ThoughtType CurrentThoughtType { get; private set; }

	public EnemyBrain(ICharacter owner)
	{
		_owner = owner;
		_idleThought = new IdleThought();
		EnterThought(ThoughtType.Idle);
	}

	public void InitializePathfindingAgent()
	{
		Agent.Initialize();
	}
	public void InitializeAttacker(AttackData attackData)
	{
		if (attackData != null)
		{
			_attackingThought = new AttackingThought(this, attackData);
		}
	}
	public void InitializeMover(MoveData moveData)
	{
		if (moveData != null)
		{
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
	public void InitializeTarget(ICharacter target)
	{
		_target = target;
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
				throw new NotImplementedException("ThoughtType not implemented: " + thoughtType);
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
	public void Attack(AttackData data, Vector3 direction)
	{
		var targetPosition = _target.GetTransformPosition().WithY(_owner.GetTransformPosition().y);
		direction = _owner.GetTransformPosition().GetDirectionTo(targetPosition);

		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = UnityEngine.Object.Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_owner.GetTransformPosition(), direction, data.ProjectileSpeed);
	}
	public void Move(MoveData data, Vector3 targetPosition)
	{
		Agent.StartPathTo(targetPosition, data.MovementSpeed, () =>
		{
		});
	}
	public void SmoothStop()
	{
		Agent.SmoothStop();
	}
	public void ClearOccupiedAgentNodes()
	{
		Agent.ClearOccupiedNodes();
	}
	public void CheckPerception()
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