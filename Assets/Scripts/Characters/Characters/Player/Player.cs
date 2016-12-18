using UnityEngine;

public class Player : Character, ICharacter
{
	private Transform _cachedTransform;
	private PlayerBrain _brain;

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

		if (Input.GetKeyDown(KeyCode.L))
		{
			Debug.Log(Inventory.GetItems().Count + " items in your inventory");
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
	public override void InitializeInventory()
	{
		Inventory = new Inventory();
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
}