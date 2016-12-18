using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemHandler
{
	[SerializeField]
	private GameObject ItemPrefab = null;

	private ItemFactory _itemFactory;

	private void Awake()
	{
		ServiceLocator<IItemHandler>.Instance = this;

		_itemFactory = new ItemFactory();
	}

	public IItem CreateRandomItem()
	{
		return _itemFactory.CreateRandomItem();
	}
	public List<IItem> CreateRandomItems(int count)
	{
		var items = new List<IItem>(count);
		for (var i = 0; i < count; i++)
		{
			items.Add(_itemFactory.CreateRandomItem());
		}

		return items;

	}
	public void CreatePhysicalItem(Vector3 position, IItem item)
	{
		var testObject = Instantiate(ItemPrefab);
		testObject.transform.position = position + Vector3.up * Random.Range(1.0f, 4.0f);
		testObject.transform.localRotation = Quaternion.Euler(0, Random.Range(0.0f, 180.0f), 0);
		testObject.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(1, 6) * 50);
		testObject.GetComponent<ItemContainer>().SetItem(item);
	}
}