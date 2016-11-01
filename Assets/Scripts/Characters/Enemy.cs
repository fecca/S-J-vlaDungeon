using System;
using UnityEngine;

[RequireComponent(typeof(Perception))]
[RequireComponent(typeof(Attacker))]
public class Enemy : Character
{
	private Perception _perception;
	private Mover _mover;
	private Attacker _attacker;
	private Player _player;
	private float _timer;
	private const float UpdateInterval = 1.0f;

	private void Awake()
	{
		_perception = GetComponent<Perception>();
		_mover = GetComponent<Mover>();
		_mover.Initialize(this);
		_attacker = GetComponent<Attacker>();
		_player = FindObjectOfType<Player>();
	}
	private void Update()
	{
		if (_timer < UpdateInterval)
		{
			_timer += Time.deltaTime;
			return;
		}
		_timer = 0f;

		Act(_perception.GetDistanceLevel(_player));
	}

	private void Act(DistanceLevel distanceLevel)
	{
		_mover.Stop();
		switch (distanceLevel)
		{
			case DistanceLevel.Outside:
				break;

			case DistanceLevel.InnerCirle:
				_attacker.Attack();
				break;

			case DistanceLevel.OuterCircle:
				_mover.Move(_player.transform.position);
				break;

			default:
				throw new NotImplementedException("Distance level not implementetd: " + distanceLevel);
		}
	}
}