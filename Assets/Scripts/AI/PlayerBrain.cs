using UnityEngine;

public class PlayerBrain : IBrain
{
	private ICharacter _owner;
	private IMover _mover;
	private IAttacker _attacker;
	private AttackData _attackData;
	private MoveData _moveData;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	public PlayerBrain(Player owner)
	{
		_owner = owner;
	}

	public void InitializeAttacker(AttackData attackData, IAttacker attacker)
	{
		if (attackData != null)
		{
			_attackData = attackData;
			_attacker = attacker;
		}
	}
	public void InitializeMover(MoveData moveData, IMover mover)
	{
		if (moveData != null)
		{
			_moveData = moveData;
			_mover = mover;
		}
	}
	public void Think()
	{
		HandleInput();
	}
	public void SmoothStop()
	{
		_mover.SmoothStop();
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
			if (hit.transform)
			{
				if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
				{
					_mover.Move(_moveData, hit.point);
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (_mouseDragTimer > _mouseDragUpdateInterval)
			{
				_mouseDragTimer = 0f;

				var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
				if (hit.transform)
				{
					if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
					{
						_mover.Move(_moveData, hit.point);
					}
				}
			}
			_mouseDragTimer += Time.deltaTime;
		}

		if (Input.GetMouseButtonDown(1))
		{
			var hit = ServiceLocator<IInputHandler>.Instance.GetRaycastHit();
			if (hit.transform != null)
			{
				var targetPosition = hit.point.WithY(_owner.GetTransformPosition().y);
				var direction = _owner.GetTransformPosition().GetDirectionTo(targetPosition);
				_attacker.Attack(_attackData, direction);
			}
		}
	}
}