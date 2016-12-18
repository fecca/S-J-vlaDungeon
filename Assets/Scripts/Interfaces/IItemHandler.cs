using System.Collections.Generic;
using UnityEngine;

public interface IItemHandler
{
	IItem CreateRandomItem();
	void CreatePhysicalItem(Vector3 position, IItem item);
	List<IItem> CreateRandomItems(int count);
}