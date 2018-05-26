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

	public IItem CreateItem(ItemType itemType)
	{
		return _itemFactory.CreateItem(itemType);
	}
	public IItem CreateRandomItem()
	{
		return _itemFactory.CreateRandomItem();
	}
	public ItemType CreateRandomItemType()
	{
		return Util.GetRandomEnumValue<ItemType>();
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
	public List<ItemType> CreateRandomItemTypes(int count)
	{
		var items = new List<ItemType>(count);
		for (var i = 0; i < count; i++)
		{
			items.Add(CreateRandomItemType());
		}

		return items;

	}
	public void CreatePhysicalItem(Vector3 position, IItem item)
	{
		var testObject = Instantiate(ItemPrefab);
		testObject.transform.position = position + Vector3.up * UnityEngine.Random.Range(1.0f, 4.0f);
		testObject.transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 180.0f), 0);
		testObject.GetComponent<Rigidbody>().AddForce(Vector3.up * UnityEngine.Random.Range(1, 6) * 50);
		testObject.GetComponent<ItemContainer>().SetItem(item);
	}
}