using UnityEngine;

public class Player : Character
{
	private Transform _cachedTransform;
	private Attacker _attacker;
	private Mover _mover;
	private float _mouseDragTimer;
	private float _mouseDragUpdateInterval = 0.1f;

	private void Awake()
	{
		_cachedTransform = transform;
		_attacker = GetComponent<Attacker>();
		_mover = GetComponent<Mover>();
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Move();
		}

		if (Input.GetMouseButtonDown(1))
		{
			Attack();
		}

		if (Input.GetMouseButton(0))
		{
			if (_mouseDragTimer > _mouseDragUpdateInterval)
			{
				_mouseDragTimer = 0f;

				Move();
			}
			_mouseDragTimer += Time.deltaTime;
		}
	}
	private void OnDrawGizmos()
	{
		Debug.DrawRay(_cachedTransform.position, _cachedTransform.forward, Color.red);
	}

	private void Move()
	{
		var hit = InputHandler.Instance.GetRaycastHit();
		if (hit.transform)
		{
			if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
			{
				_mover.Move(hit.point);
			}
		}
	}
	private void Attack()
	{
		var hit = InputHandler.Instance.GetRaycastHit();
		if (hit.transform != null)
		{
			var adjustedHitPoint = new Vector3(hit.point.x, _cachedTransform.position.y, hit.point.z);
			var direction = (adjustedHitPoint - _cachedTransform.position).normalized;
			Agent.RotateAgent(direction);
			Agent.SmoothStop();
			_attacker.Attack(direction);
		}
	}

	public override void TakeDamage()
	{
	}
}