using UnityEngine;

public abstract class Brain : ScriptableObject
{
	public virtual void Initialize(MonoBehaviour script, PathFinderAgent agent) { }
	public abstract void Think(MonoBehaviour script, PathFinderAgent agent);
}