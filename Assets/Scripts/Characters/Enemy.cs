using UnityEngine;

public class Enemy : Character
{
	private Transform _cachedTransform;
	private Brain _brain;
	private Light _pointLight;
	private Light _spotLight;

	private void Awake()
	{
		_cachedTransform = transform;
		_pointLight = _cachedTransform.FindChild("PointLight").GetComponent<Light>();
		_spotLight = _cachedTransform.FindChild("SpotLight").GetComponent<Light>();
	}
	private void Update()
	{
		if (_brain != null)
		{
			_brain.Think();
		}
	}

	public override void Attack(AttackData data, Vector3 direction)
	{
		Agent.RotateAgent(direction);
		Agent.SmoothStop();

		var projectile = Instantiate(data.ProjectilePrefab);
		projectile.GetComponent<Projectile>().Setup(_cachedTransform.position, direction, data.ProjectileSpeed);
	}
	public override void Move(MoveData data, Vector3 targetPosition)
	{
		Agent.StartPathTo(targetPosition, data.MovementSpeed, () =>
		{
		});
	}
	public void InitializeBrain()
	{
		_brain = new Brain(this);
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
		if (HealthData.CurrentHealth <= 0)
		{
			Agent.ClearNodes();
			Destroy(gameObject);
		}
	}
	public override void InitializePathfindingAgent()
	{
		var pathFinder = FindObjectOfType<PathFinder>();
		var node = pathFinder.GetRandomWalkableNode();
		transform.position = node.WorldCoordinates + Vector3.up * 5;
		Agent.Setup(pathFinder, node);
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
	public override Vector3 GetTransformPosition()
	{
		return _cachedTransform.position;
	}
}