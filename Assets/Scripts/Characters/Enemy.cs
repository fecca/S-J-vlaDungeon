using UnityEngine;

public class Enemy : Character, IAttacking, IMoving
{
	//private Canvas _canvas;
	//private RectTransform _bar;

	private Transform _cachedTransform;
	private Brain _brain;
	private Transform _target;
	private Light _pointLight;
	private Light _spotLight;

	private void Awake()
	{
		_cachedTransform = transform;
		_pointLight = _cachedTransform.FindChild("PointLight").GetComponent<Light>();
		_spotLight = _cachedTransform.FindChild("SpotLight").GetComponent<Light>();
		//_canvas = transform.FindChild("Canvas").GetComponent<Canvas>();
		//_bar = _canvas.transform.FindChild("CurrentHitpoints").GetComponent<RectTransform>();
	}
	private void Update()
	{
		if (_brain != null)
		{
			_brain.Think();
		}
		//_canvas.transform.LookAt(Camera.main.transform.position);
	}

	public void Attack(AttackData data)
	{
		var targetPosition = _target.position.WithY(_cachedTransform.position.y);
		var direction = _cachedTransform.GetDirectionTo(targetPosition);

		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, data.ProjectileSpeed);
	}
	public void Move(MoveData data)
	{
		Agent.StartPathTo(_target.position, data.MovementSpeed, () =>
		{
		});
	}

	public void InitializeBrain()
	{
		_target = FindObjectOfType<Player>().transform;
		_brain = new Brain(this, _target);
	}
	public void ActivateLights()
	{
		Debug.Log("ActivateLights");
		_pointLight.intensity = 1;
		_spotLight.intensity = 1;
	}
	public void DeactivateLights()
	{
		Debug.Log("DeactivateLights");
		_pointLight.intensity = 0;
		_spotLight.intensity = 0;
	}

	public override void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth > 0)
		{
			var fraction = HealthData.CurrentHealth / HealthData.TotalHealth;
			//_bar.sizeDelta = new Vector2(fraction * 4, _bar.sizeDelta.y);
		}
		else
		{
			Agent.ClearNodes();
			Destroy(gameObject);
		}
	}
	public override void SetHealthData(HealthData healthData)
	{
		HealthData = healthData;
	}
	public override void SetAttackData(AttackData attackData)
	{
		_brain.SetToAttacker(attackData);
	}
	public override void SetMoveData(MoveData moveData)
	{
		_brain.SetToMover(moveData);
	}
	public override void SetPerceptionData(PerceptionData perceptionData)
	{
		_brain.SetToPerceiver(perceptionData);
	}
}