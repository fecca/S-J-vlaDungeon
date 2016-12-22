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
			case ItemType.Staff:
				return new Staff((StaffTemplate)template);
			case ItemType.Wand:
				return new Wand((WandTemplate)template);
			default:
				throw new NotImplementedException("ItemType not implemented: " + itemType);
		}
	}
	public IItem CreateRandomItem()
	{
		return CreateItem(Util.GetRandomEnumValue<ItemType>());
	}
}