using UnityEngine;

public class Player : Character
{
	[SerializeField]
	private LayerMask GroundLayer;

	private Mover _mover;

	private void Awake()
	{
		_mover = GetComponent<Mover>();
		_timer = _mouseDragUpdateInterval;
	}

	private float _timer;
	private float _mouseDragUpdateInterval = 0.1f;
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Agent.SmoothStop();
		}

		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
			{
				_mover.Move(hit.point);
			}

			return;
		}

		if (Input.GetMouseButton(0))
		{
			if (_timer > _mouseDragUpdateInterval)
			{
				_timer = 0f;

				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
				{
					_mover.Move(hit.point);
				}
			}
			_timer += Time.deltaTime;

			return;
		}
	}
}