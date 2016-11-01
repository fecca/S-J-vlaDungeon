using UnityEngine;

public class Perception : MonoBehaviour
{
	[SerializeField]
	private PerceptionData Data;
	[SerializeField]
	private LayerMask Layer;

	public DistanceLevel GetDistanceLevel(Player player)
	{
		var playerPosition = player.transform.position;
		var position = transform.position;
		var ray = new Ray(position, (playerPosition - position).normalized);
		RaycastHit hit;
		Debug.DrawRay(ray.origin, ray.direction * float.MaxValue, Color.green, 1.0f);
		if (Physics.Raycast(ray, out hit, float.MaxValue))
		{
			if (hit.collider.GetComponent<Player>() != null)
			{
				var distance = Vector3.Distance(playerPosition, position);
				if (distance < Data.InnerRadius)
				{
					return DistanceLevel.InnerCirle;
				}
				else if (distance < Data.OuterRadius)
				{
					return DistanceLevel.OuterCircle;
				}
			}
		}
		return DistanceLevel.Outside;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.9f, 0.9f, 0.9f, 0.9f);
		Gizmos.DrawWireSphere(transform.position, Data.InnerRadius);
		Gizmos.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
		Gizmos.DrawWireSphere(transform.position, Data.OuterRadius);
	}
}