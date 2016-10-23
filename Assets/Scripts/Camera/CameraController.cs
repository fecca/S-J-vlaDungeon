using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float MovementSpeed = 10;

	private PlayerController _player;

	private void Start()
	{
		_player = FindObjectOfType<PlayerController>();
	}
	private void Update()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			Camera.main.orthographicSize -= 1f;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			Camera.main.orthographicSize += 1f;
		}
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5, 20);

		transform.position = Vector3.Lerp(transform.position, _player.transform.position + Vector3.up * 250f + Vector3.forward * -250, Time.deltaTime * MovementSpeed);
	}
}