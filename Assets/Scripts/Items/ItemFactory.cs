using System;

public class ItemFactory
{
	private ItemDatabase _itemDatabase;

	public ItemFactory()
	{
		_itemDatabase = new ItemDatabase();
	}

	public IItem CreateItem(ItemType itemType)
	{
		var template = _itemDatabase.GetTemplate(itemType);
		switch (itemType)
		{
			case ItemType.Rifle:
				return new Rifle((RifleTemplate)template);
			case ItemType.Pistol:
				return new Pistol((PistolTemplate)template);
			default:
				throw new NotImplementedException("ItemType not implemented: " + itemType);
		}
	}
	public IItem CreateRandomItem()
	{
		return CreateItem(Util.GetRandomEnumValue<ItemType>());
	}
}