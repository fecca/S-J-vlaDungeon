using UnityEngine;

public class Player : Character
{
	[SerializeField]
	private LayerMask GroundLayer;

	private Mover _mover;

	private void Awake()
	{
		_mover = GetComponent<Mover>();
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
			{
				_mover.Move(hit.point);
			}
		}
	}
}