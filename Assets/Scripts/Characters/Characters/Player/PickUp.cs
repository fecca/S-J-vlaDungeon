using UnityEngine;

public class PickUp : MonoBehaviour
{
	public void OnTriggerEnter(Collider collider)
	{
		var itemContainer = collider.gameObject.GetComponentInParent<ItemContainer>();
		if (itemContainer != null)
		{
			var player = GetComponent<Player>();
			if (player != null)
			{
				player.Inventory.AddItem(itemContainer.Item);
				Destroy(itemContainer.gameObject);
			}
		}
	}
}