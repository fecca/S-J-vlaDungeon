using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(MeshGenerator))]
[RequireComponent(typeof(PathFinder))]
[RequireComponent(typeof(MessageHub))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(CharacterHandler))]
public class Game : MonoBehaviour
{
	private void Awake()
	{
		MessageHub.Instance.Subscribe<CharactersSpawnedEvent>(OnCharactersSpawnedEvent);
	}
	private void Start()
	{
		MessageHub.Instance.Publish(new GameStartedEvent(null));
	}

	private void OnCharactersSpawnedEvent(CharactersSpawnedEvent charactersSpawnedEvent)
	{
		MessageHub.Instance.Publish(new GameInitializedEvent(null));
	}
}