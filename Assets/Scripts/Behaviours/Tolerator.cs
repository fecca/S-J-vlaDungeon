using System;
using UnityEngine;

public class Tolerator : MonoBehaviour, ITolerator
{
	[SerializeField]
	private ToleratorData Data;

	private Character _character;
	private Player _player;
	private float _timer = 0f;
	private float _time = 1f;
	private bool _isInInnerCircle;
	private bool _isInOuterCircle;

	public bool IsInInnerCirle { get { return _isInInnerCircle; } }
	public bool IsInOuterCirle { get { return _isInOuterCircle; } }

	public void Initialize(Character character)
	{
		_character = character;
		_player = FindObjectOfType<Player>();
	}

	public void BehaviourUpdate()
	{
		if (_timer < _time)
		{
			_timer += Time.deltaTime;
			return;
		}

		Detect();
	}

	public void Detect()
	{
		var ray = new Ray(_character.transform.position, _player.transform.position - _character.transform.position);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Vector3.Distance(_player.transform.position, _character.transform.position)))
		{
			if (hit.collider.gameObject.GetComponent<Player>() != null)
			{
				_isInInnerCircle = hit.distance < Data.InnerCirleRadius;
				_isInOuterCircle = hit.distance < Data.OuterCirleRadius && !_isInInnerCircle;
				return;
			}
			_isInInnerCircle = false;
			_isInOuterCircle = false;
		}
	}
}