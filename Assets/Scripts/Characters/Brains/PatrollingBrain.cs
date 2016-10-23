using UnityEngine;

[CreateAssetMenu(menuName = "Brains/Enemies/Patroller")]
public class PatrollingEnemy : Brain
{
	[SerializeField]
	private float MovementSpeed = 20.0f;

	private MapGenerator _mapGenerator;

	public override void Initialize(MonoBehaviour script, PathFinderAgent agent)
	{
		agent.Setup(FindObjectOfType<PathFinder>());
		_mapGenerator = FindObjectOfType<MapGenerator>();
	}
	public override void Think(MonoBehaviour script, PathFinderAgent agent)
	{
		if (!agent.IsMoving)
		{
			agent.StartPathTo(_mapGenerator.GetRandomWalkableTile(), MovementSpeed, () =>
			{
			});
		}
	}
}