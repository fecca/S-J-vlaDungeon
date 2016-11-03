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
		EnemyState state;
		var targetPosition = _perception.GetDistanceLevel(_targetTransform.position);
		switch (targetPosition)
		{
			case PlayerPosition.Outside:
				state = EnemyState.Idle;
				break;
			case PlayerPosition.InnerCirle:
				state = EnemyState.Attacking;
				break;
			case PlayerPosition.OuterCircle:
				state = EnemyState.Moving;
				break;
			default:
				throw new NotImplementedException("PlayerPosition type not implemented: " + targetPosition);
		}

		SwitchState(state);
	}

	private void SwitchState(EnemyState state)
	{
		IBehaviour nextBehaviour;
		switch (state)
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
				throw new NotImplementedException("EnemyState type not implemented: " + state);
		}

		if (nextBehaviour != _currentBehaviour)
		{
			_currentBehaviour.Stop();
			_currentBehaviour = nextBehaviour;
			_currentBehaviour.Start();
		}
	}
}