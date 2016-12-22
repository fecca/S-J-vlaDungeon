using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, ICharacter
{
	private Transform _cachedTransform;
	private EnemyBrain _brain;
	private List<ItemType> _loot;

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
	public void InitializeLoot()
	{
		_loot = new List<ItemType>();
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
		if (_loot.IsEmpty())
		{
			return;
		}

		var availableNeighbouringNodes = ServiceLocator<IPathFinder>.Instance.GetAvailableNeighouringNodes(_cachedTransform.position);
		if (availableNeighbouringNodes.Count < _loot.Count)
		{
			throw new ArgumentException("Not enough available nodes to drop items on");
		}

		for (var i = 0; i < _loot.Count; i++)
		{
			var item = ServiceLocator<IItemHandler>.Instance.CreateItem(_loot[i]);
			var randomNeighbour = availableNeighbouringNodes.GetRandomElement();
			ServiceLocator<IItemHandler>.Instance.CreatePhysicalItem(randomNeighbour.WorldCoordinates, item);
			availableNeighbouringNodes.Remove(randomNeighbour);

			Debug.Log("Dropped: " + item);
		}

		_loot.Clear();
	}
	private void AddLoot()
	{
		var numberOfItems = 0;
		var randomNumber = UnityEngine.Random.Range(0, 10);
		switch (randomNumber)
		{
			case 9:
				numberOfItems = 2;
				break;
			case 8:
			case 7:
			case 6:
			case 5:
				numberOfItems = 1;
				break;
			case 4:
			case 3:
			case 2:
			case 1:
			case 0:
				numberOfItems = 0;
				break;
		}
		var items = ServiceLocator<IItemHandler>.Instance.CreateRandomItemTypes(numberOfItems);
		_loot.AddRange(items);
	}
}