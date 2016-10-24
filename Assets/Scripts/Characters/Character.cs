using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
	protected List<IBehaviour> _behaviours;

	private void Awake()
	{
		_behaviours = new List<IBehaviour>(8);
	}

	public void RegisterBehaviour(IBehaviour behaviour)
	{
		_behaviours.Add(behaviour);
		Debug.Log("Behaviour(" + behaviour.GetType() + ") was added");
	}
	public void RemoveBehaviour(IBehaviour behaviour)
	{
		_behaviours.Remove(behaviour);
		Debug.Log("Behaviour(" + behaviour.GetType() + ") was removed");
	}
}