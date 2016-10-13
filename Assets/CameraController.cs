using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private GameObject Player = null;

	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, Player.transform.position + Vector3.up * 250f + Vector3.forward * -250, Time.deltaTime * 1.5f);
	}
}