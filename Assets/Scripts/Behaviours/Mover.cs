using UnityEngine;

public class Mover : MonoBehaviour, IMover
{
	[SerializeField]
	private MoverData Data;

	private Character _character;

	public void Awake()
	{
		_character = GetComponent<Character>();
	}

	public void Move(Vector3 position)
	{
		_character.Agent.StartPathTo(position, Data.MovementSpeed, () =>
		{
		});
	}

	public void Stop()
	{
		_character.Agent.Stop();
	}
}