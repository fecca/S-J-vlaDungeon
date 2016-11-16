using UnityEngine;

public class Player : Character
{
	private Attacker _attacker;
	private Mover _mover;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	private void Awake()
	{
		_attacker = GetComponent<Attacker>();
		_mover = GetComponent<Mover>();
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = InputHandler.Instance.GetRaycastHit();
			if (hit.transform)
			{
				if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
				{
					_mover.Move(hit.point);
				}
			}

			return;
		}

		if (Input.GetMouseButtonDown(1))
		{
			var hit = InputHandler.Instance.GetRaycastHit();
			if (hit.transform)
			{
				_attacker.Attack(hit.point);
			}

			return;
		}

		if (Input.GetMouseButton(0))
		{
			if (_mouseDragTimer > _mouseDragUpdateInterval)
			{
				_mouseDragTimer = 0f;

				var hit = InputHandler.Instance.GetRaycastHit();
				if (hit.transform)
				{
					if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
					{
						_mover.Move(hit.point);
					}
				}
			}
			_mouseDragTimer += Time.deltaTime;

			return;
		}
	}

	public override void TakeDamage()
	{
	}
}