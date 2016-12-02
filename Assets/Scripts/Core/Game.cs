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
		MessageHub.Instance.Subscribe<CharactersCreatedEvent>(OnCharactersCreatedEvent);
		MessageHub.Instance.Subscribe<MapDestroyedEvent>(OnMapDestroyedEvent);
	}
	private void Start()
	{
		MessageHub.Instance.Publish(new CreateGameEvent(null));
	}

	private void OnCharactersCreatedEvent(CharactersCreatedEvent charactersCreatedEvent)
	{
		MessageHub.Instance.Publish(new GameCreatedEvent(null));
	}
	private void OnMapDestroyedEvent(MapDestroyedEvent mapDestroyedEvent)
	{
		MessageHub.Instance.Publish(new CreateGameEvent(null));
	}
}