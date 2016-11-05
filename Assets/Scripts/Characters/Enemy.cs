using System;
using UnityEngine;

[RequireComponent(typeof(Perception))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Idle))]
public class Enemy : Character
{
	private Perception _perception;
	private Mover _mover;
	private Idle _idle;
	private Attacker _attacker;
	private Transform _targetTransform;
	private float _timer;
	private IBehaviour _currentBehaviour;
	private EnemyState _currentState;

	private void Awake()
	{
		_perception = GetComponent<Perception>();
		_mover = GetComponent<Mover>();
		_idle = GetComponent<Idle>();
		_attacker = GetComponent<Attacker>();
		_targetTransform = FindObjectOfType<Player>().transform;
		_currentBehaviour = _idle;
	}
	private void Update()
	{
		if (_timer > _perception.GetUpdateInterval())
		{
			_timer = 0f;
			UpdateState();
		}

		_currentBehaviour.UpdateBehaviour(_targetTransform);

		_timer += Time.deltaTime;
	}

	private void UpdateState()
	{
		EnemyState nextState;
		var targetPosition = _perception.GetPlayerPosition(_targetTransform.position);
		switch (targetPosition)
		{
			case PlayerPosition.Outside:
				nextState = EnemyState.Idle;
				break;

			case PlayerPosition.InnerCirle:
				nextState = EnemyState.Attacking;
				break;

			case PlayerPosition.OuterCircle:
				nextState = EnemyState.Moving;
				break;

			case PlayerPosition.BehindWall:
				if (_currentState == EnemyState.Moving || _currentState == EnemyState.Attacking)
				{
					nextState = EnemyState.Moving;
				}
				else
				{
					nextState = EnemyState.Idle;
				}
				break;

			default:
				throw new NotImplementedException("PlayerPosition type not implemented: " + targetPosition);
		}

		_currentState = nextState;
		SwitchState();
	}

	private void SwitchState()
	{
		IBehaviour nextBehaviour;
		switch (_currentState)
		{
			case EnemyState.Idle:
				nextBehaviour = _idle;
				break;

			case EnemyState.Moving:
				nextBehaviour = _mover;
				break;

			case EnemyState.Attacking:
				nextBehaviour = _attacker;
				break;

			default:
				throw new NotImplementedException("EnemyState type not implemented: " + _currentState);
		}

		if (nextBehaviour != _currentBehaviour)
		{
			_currentBehaviour.Stop();
			_currentBehaviour = nextBehaviour;
			_currentBehaviour.Start();
		}
	}
}