using UnityEngine;

public class Mover : MonoBehaviour, IMover
{
	[SerializeField]
	private MoverData Data = null;

	private Character _character;
	private float _timer;

	private void Awake()
	{
		_character = GetComponent<Character>();
	}

	public void Start()
	{
		_timer = Data.PositionUpdateInterval;
	}
	public void Stop()
	{
		_character.Agent.SmoothStop();
	}
	public void UpdateBehaviour(Transform targetTransform)
	{
		if (_timer > Data.PositionUpdateInterval)
		{
			_timer = 0;
			Move(targetTransform.position);
		}

		_timer += Time.deltaTime;
	}

	public void Move(Vector3 position)
	{
		_character.Agent.StartPathTo(position, Data.MovementSpeed, () =>
		{
		});
	}
}