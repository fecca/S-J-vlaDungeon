using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private GameObject Player = null;

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

		transform.position = Vector3.Lerp(transform.position, Player.transform.position + Vector3.up * 250f + Vector3.forward * -250, Time.deltaTime * 1.5f);
	}
}