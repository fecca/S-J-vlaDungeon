using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private PlayerBrain Brain;

	private PathFinderAgent _agent;

	private void Start()
	{
		_agent = GetComponent<PathFinderAgent>();
		Brain.Initialize(this, _agent);
	}
	private void Update()
	{
		if (Brain == null)
		{
			return;
		}

		Brain.Think(this, _agent);
	}
}