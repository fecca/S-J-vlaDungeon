using System;
using UnityEngine;

public class Enemy : Character, ICharacter
{
	private Transform _cachedTransform;
	private EnemyBrain _brain;

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
		_brain = new EnemyBrain(this);
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
		AddLoot();
	}
	public void InitializePerception(PerceptionData perceptionData)
	{
		_brain.InitializePerception(perceptionData);
	}
	public void InitializeTarget(ICharacter target)
	{
		_brain.InitializeTarget(target);
	}

	public override void TakeDamage()
	{
		HealthData.CurrentHealth--;
		if (HealthData.CurrentHealth <= 0)
		{
			_brain.ClearOccupiedAgentNodes();
			DropItems();
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

	private void DropItems()
	{
		if (!Inventory.HasItems())
		{
			return;
		}

		var items = Inventory.GetItems();
		var availableNeighbouringNodes = ServiceLocator<IPathFinder>.Instance.GetAvailableNeighouringNodes(_cachedTransform.position);
		if (availableNeighbouringNodes.Count < items.Count)
		{
			throw new ArgumentException("Not enough available nodes to drop items on");
		}

		for (var i = 0; i < items.Count; i++)
		{
			var randomNeighbour = availableNeighbouringNodes.GetRandomElement();
			ServiceLocator<IItemHandler>.Instance.CreatePhysicalItem(randomNeighbour.WorldCoordinates, items[i]);
			availableNeighbouringNodes.Remove(randomNeighbour);

			Debug.Log("Dropped: " + items[i]);
		}

		Inventory.RemoveAllItems();
	}
	private void AddLoot()
	{
		var numberOfItems = 0;
		var randomNumber = UnityEngine.Random.Range(0, 10);
		switch (randomNumber)
		{
			case 9:
			case 8:
				numberOfItems = 2;
				break;
			case 7:
			case 6:
			case 5:
			case 4:
				numberOfItems = 1;
				break;
			case 3:
			case 2:
			case 1:
			case 0:
				numberOfItems = 0;
				break;
		}
		var items = ServiceLocator<IItemHandler>.Instance.CreateRandomItems(numberOfItems);
		Inventory.AddItems(items);
	}
}