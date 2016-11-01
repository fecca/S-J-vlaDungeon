using UnityEngine;

public class Perception : MonoBehaviour
{
	[SerializeField]
	private PerceptionData Data;

	public DistanceLevel GetDistanceLevel(Vector3 playerPosition)
	{
		var position = transform.position;
		var ray = new Ray(position, (playerPosition - position).normalized);

		Debug.DrawRay(ray.origin, ray.direction * float.MaxValue, Color.green, 0.5f);

		RaycastHit hit;
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
	public float GetUpdateInterval()
	{
		return Data.UpdateInterval;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.9f, 0.9f, 0.9f, 0.9f);
		Gizmos.DrawWireSphere(transform.position, Data.InnerRadius);
		Gizmos.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
		Gizmos.DrawWireSphere(transform.position, Data.OuterRadius);
	}
}