using System;
using UnityEngine;

[RequireComponent(typeof(Perception))]
public abstract class Enemy : Character
{
	private Perception _perception;
	private PerceptionLevel _perceptionLevel;
	private float _timer;
	private const float UpdateInterval = 1.0f;

	private void Awake()
	{
		_perception = GetComponent<Perception>();
	}
	private void Update()
	{
		if (_timer < UpdateInterval)
		{
			_timer += Time.deltaTime;
			return;
		}
		_timer = 0f;

		_perceptionLevel = _perception.GetPlayerDistance();
	}

	private void Act()
	{
		switch (_perceptionLevel)
		{
			case PerceptionLevel.Outside:
				break;

			case PerceptionLevel.InnerCirle:
				break;

			case PerceptionLevel.OuterCircle:
				break;

			default:
				throw new NotImplementedException("Perception level not implementetd: " + _perceptionLevel);
		}
	}
}

public enum PerceptionLevel
{
	Outside,
	InnerCirle,
	OuterCircle
}

public class Perception : MonoBehaviour
{
	private PerceptionData Data;

	public PerceptionLevel GetPlayerDistance()
	{
		return PerceptionLevel.Outside;
	}
}

public class PerceptionData : ScriptableObject
{
	public const float InnerRadius = 10.0f;
	public const float OuterRadius = 10.0f;
}