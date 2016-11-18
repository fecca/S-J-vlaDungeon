using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	private Vector3 _direction;
	private float _speed;

	private void Update()
	{
		transform.Translate(_direction * Time.deltaTime * _speed, Space.World);
	}
	private void OnTriggerEnter(Collider collider)
	{
		Destroy(gameObject);
		var character = collider.gameObject.GetComponent<Character>();
		if (character != null)
		{
			character.TakeDamage();
		}
	}

	public void Setup(Vector3 spawnPosition, Vector3 direction, float speed)
	{
		_direction = direction;
		_speed = speed;

		transform.position = spawnPosition + direction;
	}
}