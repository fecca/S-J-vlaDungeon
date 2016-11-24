using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float MovementSpeed = 10;

	private Transform _cachedTransform;
	private Player _player;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<CharactersSpawnedEvent>(OnCharacterSpawnedEvent);
		_cachedTransform = transform;
	}
	private void OnCharacterSpawnedEvent(CharactersSpawnedEvent charactersSpawnedEvent)
	{
		_player = FindObjectOfType<Player>();
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

		if (_player != null)
		{
			_cachedTransform.position = Vector3.Lerp(_cachedTransform.position, _player.transform.position + Vector3.up * 25f + Vector3.forward * -25, Time.deltaTime * MovementSpeed);
		}
	}
}