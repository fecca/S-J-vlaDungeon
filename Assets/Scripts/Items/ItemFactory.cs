using System;

public class ItemFactory
{
	public IItem CreateItem(ItemType itemType)
	{
		switch (itemType)
		{
			case ItemType.Rifle:
				return new Rifle();
			case ItemType.Pistol:
				return new Pistol();
			default:
				throw new NotImplementedException("ItemType not implemented: " + itemType);
		}
	}
	public IItem CreateRandomItem()
	{
		var itemType = Util.GetRandomEnumValue<ItemType>();
		switch (itemType)
		{
			case ItemType.Rifle:
				return new Rifle();
			case ItemType.Pistol:
				return new Pistol();
			default:
				throw new NotImplementedException("ItemType not implemented: " + itemType);
		}
	}
}