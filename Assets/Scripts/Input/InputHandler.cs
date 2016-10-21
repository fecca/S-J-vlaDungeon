using UnityEngine;

public class InputHandler : MonoBehaviour
{
	[SerializeField]
	private LayerMask GroundLayer = 0;

	private PlayerController _player;

	private void Start()
	{
		_player = FindObjectOfType<PlayerController>();
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f, GroundLayer))
			{
				var targetPosition = new Vector2(hit.point.x, hit.point.z);
				_player.ClickedGround(targetPosition);
			}
		}
	}
}