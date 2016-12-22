using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject WandObject = null;
	[SerializeField]
	private GameObject StaffObject = null;

	private GameObject _activeObject;

	public IItem Item { get; private set; }

	private void Update()
	{
		transform.Rotate(Vector3.up, Time.deltaTime * 90, Space.Self);
	}
	private void OnMouseEnter()
	{
		var renderers = _activeObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.color += new Color(0.2f, 0.5f, 0.2f);
		}
	}
	private void OnMouseExit()
	{
		var renderers = _activeObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.color -= new Color(0.2f, 0.5f, 0.2f);
		}
	}

	public void SetItem(IItem item)
	{
		Item = item;
		if (item is Staff)
		{
			_activeObject = StaffObject;
		}
		else
		{
			_activeObject = WandObject;
		}
		_activeObject.SetActive(true);
	}
}