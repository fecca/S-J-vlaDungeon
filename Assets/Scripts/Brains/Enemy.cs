using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class Enemy : MonoBehaviour
{
	[SerializeField]
	private EnemyBrain Brain;

	private PathFinderAgent _pathFinderAgent;

	private void Start()
	{
		_pathFinderAgent = GetComponent<PathFinderAgent>();
		Brain.Initialize(this, _pathFinderAgent);
	}
	private void Update()
	{
		if (Brain == null)
		{
			return;
		}

		Brain.Think(this, _pathFinderAgent);
	}
}