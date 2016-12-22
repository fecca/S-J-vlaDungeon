using System.Collections.Generic;
using UnityEngine;

public interface IItemHandler
{
	IItem CreateItem(ItemType itemType);
	IItem CreateRandomItem();
	ItemType CreateRandomItemType();
	List<IItem> CreateRandomItems(int count);
	List<ItemType> CreateRandomItemTypes(int count);
	void CreatePhysicalItem(Vector3 position, IItem item);
}