using UnityEngine;

public class Perception : MonoBehaviour
{
	[SerializeField]
	private PerceptionData Data = null;

	public PlayerPosition GetPlayerPosition(Vector3 targetPosition)
	{
		var position = transform.position;
		var ray = new Ray(position, (targetPosition - position).normalized);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue))
		{
			if (hit.collider.GetComponent<Player>() != null)
			{
				var distance = Vector3.Distance(targetPosition, position);
				if (distance < Data.InnerRadius)
				{
					return PlayerPosition.InnerCirle;
				}

				if (distance < Data.OuterRadius)
				{
					return PlayerPosition.OuterCircle;
				}

				return PlayerPosition.Outside;
			}

			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				return PlayerPosition.BehindWall;
			}
		}
		return PlayerPosition.Outside;
	}
	public float GetRandomUpdateInterval()
	{
		return Data.UpdateInterval * Random.Range(0.8f, 1.2f);
	}
}