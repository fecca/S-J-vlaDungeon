using System.Collections.Generic;
using UnityEngine;

public class PathFinderAgent : MonoBehaviour
{
	[SerializeField]
	private float MovementSpeed = 5.0f;

	private LinkedList<PathfindingNode> _path;
	private PathFinder _pathFinder;
	private Color _randomColor;

	public void Setup(PathFinder pathFinder)
	{
		_path = new LinkedList<PathfindingNode>();
		_pathFinder = pathFinder;
		_randomColor = new Color(Random.value, Random.value, Random.value);
	}

	public void StartPath(Vector2 from, Vector2 to)
	{
		PathfindingNode unfinishedNode = null;
		if (_path.Count > 0)
		{
			unfinishedNode = _path.First.Value;
		}

		//System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		//stopWatch.Start();

		_path = _pathFinder.GetPath(from, to);

		//stopWatch.Stop();
		//Debug.Log("GetPath() took " + stopWatch.ElapsedMilliseconds + "ms");

		if (unfinishedNode != null)
		{
			_path.AddFirst(unfinishedNode);
		}
	}

	private void MoveAlongPath()
	{
		var targetNode = _path.First;

		var targetPosition = new Vector3(targetNode.Value.WorldCoordinates.X, transform.position.y, targetNode.Value.WorldCoordinates.Y);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * MovementSpeed);

		if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
		{
			_path.Remove(targetNode);
		}
	}

	private void FixedUpdate()
	{
		if (_path.Count > 0)
		{
			MoveAlongPath();
		}
	}
	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawCube(new Vector3(iteration.Value.WorldCoordinates.X, 1, iteration.Value.WorldCoordinates.Y), Vector3.one * 0.25f);
			}
		}
	}
}