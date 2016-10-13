using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private LayerMask GroundLayer;

	private PathFinder _pathFinder;
	private LinkedList<PathNode> _path;

	private void Update()
	{
		//if (Input.GetMouseButtonDown(0))
		//{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
			{
				var point = new Vector2((int)hit.point.x, (int)hit.point.z);
				var playerPosition = new Vector2((int)transform.position.x, (int)transform.position.z);
				_path = _pathFinder.GetPath(playerPosition, point);
			}
		//}
	}

	public void Initialize(Vector3 position, PathFinder pathFinder)
	{
		position.y = 5;
		transform.position = position;

		_pathFinder = pathFinder;
	}

	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			for (var iteration = _path.First; iteration != null; iteration = iteration.Next)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(new Vector3(iteration.Value.Tile.Coordinates.X, 0, iteration.Value.Tile.Coordinates.Y), Vector3.one * 0.25f);
			}
		}
	}
}