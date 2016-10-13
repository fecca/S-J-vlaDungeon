using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public void SetPosition(Vector3 position)
	{
		position.y = 5;
		transform.position = position;
	}
}