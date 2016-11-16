using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	private Vector3 _targetPosition;
	private float _speed;

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
		if (transform.position == _targetPosition)
		{
			Destroy(gameObject);
		}
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

	public void Setup(Vector3 spawnPosition, Vector3 targetPosition, float speed)
	{
		_targetPosition = targetPosition;
		_speed = speed;

		var direction = (targetPosition - spawnPosition).normalized;
		transform.position = spawnPosition + direction;
	}
}