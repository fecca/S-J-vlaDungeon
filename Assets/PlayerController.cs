using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(1))
		{
			GetComponent<Seeker>().StartPath(transform.position, transform.position + Vector3.right * 5, (message) => { });
		}
	}
}