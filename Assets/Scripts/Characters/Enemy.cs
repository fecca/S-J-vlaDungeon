using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Tolerator))]
public class Enemy : Character
{
	private IMover _mover;
	private IAttacker _attacker;
	private ITolerator _detector;

	private void Awake()
	{
		_mover = GetComponent<Mover>();
		_attacker = GetComponent<Attacker>();
		_detector = GetComponent<Tolerator>();

		_mover.Initialize(this);
		_attacker.Initialize(this);
		_detector.Initialize(this);
	}

	private void Update()
	{
		_detector.BehaviourUpdate();
		if (_detector.IsInInnerCirle)
		{
			_attacker.BehaviourUpdate();
		}
		else if (_detector.IsInOuterCirle)
		{
			_mover.BehaviourUpdate();
		}
	}
}