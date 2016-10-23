using UnityEngine;

[RequireComponent(typeof(PathFinderAgent))]
public class Enemy : MonoBehaviour
{
	private float _timer;
	private const float NewPathTime = 5f;
	private PathFinderAgent _agent;
	private MapGenerator _mapGenerator;

	private void Start()
	{
		_mapGenerator = FindObjectOfType<MapGenerator>();
		_agent = GetComponent<PathFinderAgent>();
		_agent.Setup(FindObjectOfType<PathFinder>());
		_timer = NewPathTime;
	}
	private void Update()
	{
		if (_timer < NewPathTime)
		{
			_timer += Time.deltaTime;
			return;
		}
		_timer = 0;

		_agent.StartPathTo(_mapGenerator.GetRandomWalkableTile(), () =>
		{
			_timer = NewPathTime;
		});
	}
}