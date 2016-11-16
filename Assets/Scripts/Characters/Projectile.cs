using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(gameObject);
		var character = collision.gameObject.GetComponent<Character>();
		if (character != null)
		{
			character.TakeDamage();
		}
	}
}