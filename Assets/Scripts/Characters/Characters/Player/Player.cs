using UnityEngine;

public class Player : Character
{
	private Transform _cachedTransform;
	private PlayerBrain _brain;
	private Inventory _inventory;

	private void Awake()
	{
		_cachedTransform = transform;
	}
	private void Update()
	{
		if (_brain != null)
		{
			_brain.Think();
		}
	}
	private void OnTriggerEnter(Collider collider)
	{
		var projectile = collider.gameObject.GetComponent<Projectile>();
		if (projectile != null)
		{
			TakeDamage();
		}
	}

	public override void InitializeBrain()
	{
		_brain = new PlayerBrain(this);
	}
	public override void InitializePathfindingAgent()
	{
		_brain.InitializePathfindingAgent();
	}
	public override void InitializeHealth(HealthData healthData)
	{
		HealthData = healthData;
	}
	public override void InitializeAttacker(AttackData attackData)
	{
		_brain.InitializeAttacker(attackData);
	}
	public override void InitializeMover(MoveData moveData)
	{
		_brain.InitializeMover(moveData);
	}
	public override void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth <= 0)
		{
			Destroy(gameObject);
		}
	}
	public override GameObject GetGameObject()
	{
		return gameObject;
	}
	public override Vector3 GetTransformPosition()
	{
		return _cachedTransform.position;
	}

	public void InitializeInventory()
	{
		_inventory = new Inventory();
	}
	public void AddItemToInventory(IItem item)
	{
		_inventory.AddItem(item);
	}
}