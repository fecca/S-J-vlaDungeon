using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	private Transform _cachedTransform;
	private Vector3 _direction;
	private float _speed;

	private void Awake()
	{
		_cachedTransform = transform;
	}

	private void FixedUpdate()
	{
		_cachedTransform.Translate(_direction * Time.deltaTime * _speed, Space.World);
	}
	private void OnTriggerEnter(Collider collider)
	{
		Destroy(gameObject);
	}

	public void Setup(Vector3 spawnPosition, Vector3 direction, float speed)
	{
		_direction = direction;
		_speed = speed;

		_cachedTransform.position = spawnPosition + direction;
	}
}