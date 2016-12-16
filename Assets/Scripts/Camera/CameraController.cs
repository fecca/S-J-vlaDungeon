using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float FollowSpeed = 10;
	[SerializeField]
	private float RotationSpeed = 1;

	private Transform _cachedTransform;
	private ICharacter _player;

	private void Awake()
	{
		MessageHub.Instance.Subscribe<CharactersCreatedEvent>(OnCharactersCreatedEvent);
		_cachedTransform = transform;
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
			if (Input.GetMouseButton(2))
			{
				_cachedTransform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * RotationSpeed);
			}
			_cachedTransform.position = Vector3.Lerp(_cachedTransform.position, _player.GetTransformPosition(), Time.deltaTime * FollowSpeed);
		}
	}

	private void OnCharactersCreatedEvent(CharactersCreatedEvent charactersCreatedEvent)
	{
		_player = ServiceLocator<ICharacterHandler>.Instance.GetPlayer();
	}
}