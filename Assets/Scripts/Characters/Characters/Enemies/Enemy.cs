using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
	private Transform _cachedTransform;
	private EnemyBrain _brain;
	private Dictionary<ItemType, int> _loot;

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
		_loot = new Dictionary<ItemType, int>();
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
			var loot = _loot.ElementAt(i);
			for (int j = 0; j < loot.Value; j++)
			{
				var item = ServiceLocator<IItemHandler>.Instance.CreateItem(loot.Key);
				var randomNeighbour = availableNeighbouringNodes.GetRandomElement();
				ServiceLocator<IItemHandler>.Instance.CreatePhysicalItem(randomNeighbour.WorldCoordinates, item);
				availableNeighbouringNodes.Remove(randomNeighbour);

				Debug.Log("Dropped: " + item);
			}
		}

		_loot.Clear();
	}
	private void AddLoot()
	{
		var numberOfItems = 0;
		switch (UnityEngine.Random.Range(0, 10))
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
		for (var i = 0; i < items.Count; i++)
		{
			if (_loot.ContainsKey(items[i]))
			{
				_loot[items[i]]++;
			}
			else
			{
				_loot.Add(items[i], 1);
			}
		}
	}
}