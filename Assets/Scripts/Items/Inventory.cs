using System.Collections.Generic;

public class Inventory
{
	private List<IItem> _items;

	public Inventory()
	{
		_items = new List<IItem>(128);
	}

	public void AddItem(IItem item)
	{
		_items.Add(item);
	}
	public void AddItems(List<IItem> items)
	{
		_items.AddRange(items);
	}
	public void RemoveItem(IItem item)
	{
		_items.Remove(item);
	}
	public void RemoveAllItems()
	{
		_items.Clear();
	}
	public bool HasItems()
	{
		return _items != null && _items.Count > 0;
	}
	public List<IItem> GetItems()
	{
		return new List<IItem>(_items);
	}
}