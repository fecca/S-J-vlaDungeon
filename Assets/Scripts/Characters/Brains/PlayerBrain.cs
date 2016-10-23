using UnityEngine;

[CreateAssetMenu(menuName = "Brains/Player")]
public class PlayerBrain : Brain
{
	[SerializeField]
	private float MovementSpeed = 30.0f;

	public override void Initialize(MonoBehaviour script, PathFinderAgent agent)
	{
		agent.Setup(FindObjectOfType<PathFinder>());
	}
	public override void Think(MonoBehaviour script, PathFinderAgent agent)
	{
		if (Input.GetMouseButtonUp(0))
		{
			var hitPoint = InputHandler.Instance.GetHitPoint(Input.mousePosition);
			if (hitPoint.magnitude >= 0)
			{
				agent.StartPathTo(hitPoint, MovementSpeed, () =>
				{
					Debug.Log("Arrived at target");
				});
			}
		}
	}
}