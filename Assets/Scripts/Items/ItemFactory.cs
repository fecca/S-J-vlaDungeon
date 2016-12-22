using System;

public class ItemFactory
{
	public IItem CreateItem(ItemDatabase itemDatabase, ItemType itemType)
	{
		var template = itemDatabase.GetTemplate(itemType);
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
	public IItem CreateRandomItem(ItemDatabase itemDatabase)
	{
		return CreateItem(itemDatabase, Util.GetRandomEnumValue<ItemType>());
	}
}