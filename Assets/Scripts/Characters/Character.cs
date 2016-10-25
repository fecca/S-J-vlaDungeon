using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
	protected List<IBehaviour> _behaviours;
	protected PathFinderAgent _agent;

	public PathFinderAgent Agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = GetComponent<PathFinderAgent>();
				_agent.Setup(FindObjectOfType<PathFinder>());
			}
			return _agent;
		}
	}

	private void Awake()
	{
		_behaviours = new List<IBehaviour>(8);
	}
	private void Update()
	{
		foreach (var behaviour in _behaviours)
		{
			behaviour.BehaviourUpdate();
		}
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