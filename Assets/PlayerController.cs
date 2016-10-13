using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private LayerMask GroundLayer;

	private PathFinder _pathFinder;
	private LinkedList<PathNode> _path = new LinkedList<PathNode>();

	public void Initialize(Vector3 position, PathFinder pathFinder)
	{
		position.y = 5;
		transform.position = position;

		_pathFinder = pathFinder;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
			{
				var point = new Vector2((int)hit.point.x, (int)hit.point.z);
				var playerPosition = new Vector2(transform.position.x, transform.position.z);
				if (_path.Count > 1)
				{
					playerPosition = new Vector3(_path.First.Next.Value.Tile.Coordinates.X, transform.position.y, _path.First.Next.Value.Tile.Coordinates.Y);
				}
				_path = _pathFinder.GetPath(playerPosition, point);
			}
		}

		if (_path.Count > 0)
		{
			MoveAlongPath();
		}
	}

	private void MoveAlongPath()
	{
		var targetNode = _path.First;
		var targetPosition = new Vector3(targetNode.Value.Tile.Coordinates.X, transform.position.y, targetNode.Value.Tile.Coordinates.Y);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 20f);

		if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
		{
			_path.Remove(targetNode);
		}
	}

	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(new Vector3(iteration.Value.Tile.Coordinates.X, 1, iteration.Value.Tile.Coordinates.Y), Vector3.one * 0.5f);
			}
		}
	}
}