using UnityEngine;

public class InputHandler : MonoBehaviour
{
	[SerializeField]
	private LayerMask GroundLayer;

	private static InputHandler _instance;

	public static InputHandler Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public Vector3 GetHitPoint()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
		{
			return hit.point;
		}

		return -Vector3.one;
	}
}