using System.Collections.Generic;

public class ItemDatabase
{
	private Dictionary<ItemType, ItemTemplate> _itemTemplates = new Dictionary<ItemType, ItemTemplate>();

	public ItemDatabase()
	{
		_itemTemplates.Add(ItemType.Wand, new WandTemplate());
		_itemTemplates.Add(ItemType.Staff, new StaffTemplate());
	}

	public ItemTemplate GetTemplate(ItemType itemType)
	{
		var itemTemplate = _itemTemplates[itemType];
		if (itemTemplate == null)
		{
			throw new System.ArgumentException("Item template for type does not exist: : " + itemType);
		}

		return itemTemplate;
	}
}