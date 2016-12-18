using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject PistolObject;
	[SerializeField]
	private GameObject RifleObject;

	public IItem Item { get; private set; }

	private void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * 90, Space.Self);
	}

	public void SetItem(IItem item)
	{
		Item = item;
		if (item is Rifle)
		{
			RifleObject.SetActive(true);
		}
		else
		{
			PistolObject.SetActive(true);
		}
	}
}