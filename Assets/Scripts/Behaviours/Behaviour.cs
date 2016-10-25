using System;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour
{
	protected Character _character;
	protected void Awake()
	{
		_character = GetComponent<Character>();
		if (_character == null)
		{
			throw new NotSupportedException("Can not add script to this object.");
		}

		RegisterBehaviour();
	}
	protected abstract void RegisterBehaviour();
}