using System;
using UnityEngine;

[RequireComponent(typeof(Perception))]
[RequireComponent(typeof(Attacker))]
public class Enemy : Character
{
	private Perception _perception;
	private Mover _mover;
	private Attacker _attacker;
	private Transform _playerTransform;
	private float _timer;
	private const float UpdateInterval = 1.0f;

	private void Awake()
	{
		_perception = GetComponent<Perception>();
		_mover = GetComponent<Mover>();
		_attacker = GetComponent<Attacker>();
		_playerTransform = FindObjectOfType<Player>().transform;
	}
	private void Update()
	{
		if (_timer < UpdateInterval)
		{
			_timer += Time.deltaTime;
			return;
		}
		_timer = 0f;

		Act(_perception.GetDistanceLevel(_playerTransform.position));
	}

	private void Act(DistanceLevel distanceLevel)
	{
		_mover.Stop();
		switch (distanceLevel)
		{
			case DistanceLevel.Outside:
				break;

			case DistanceLevel.InnerCirle:
				_attacker.Attack(_playerTransform.position);
				break;

			case DistanceLevel.OuterCircle:
				_mover.Move(_playerTransform.position);
				break;

			default:
				throw new NotImplementedException("Distance level not implementetd: " + distanceLevel);
		}
	}
}