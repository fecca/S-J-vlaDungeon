using UnityEngine;

public class Perception
{
	private PerceptionData _perceptionData;

	public Perception(PerceptionData perceptionData)
	{
		_perceptionData = perceptionData;
	}

	public PerceptionState GetPerceptionState(Vector3 ownerPosition, Vector3 targetPosition)
	{
		var ray = new Ray(ownerPosition, ownerPosition.GetDirectionTo(targetPosition));
		var layerMask = (1 << 11) | (1 << 13) | (1 << 14);
		layerMask = ~layerMask;
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
		{
			if (hit.collider.GetComponent<Player>() != null)
			{
				var distance = Vector3.Distance(targetPosition, ownerPosition);
				if (distance < _perceptionData.InnerRadius)
				{
					return PerceptionState.InnerCirle;
				}

				if (distance < _perceptionData.OuterRadius)
				{
					return PerceptionState.OuterCircle;
				}

				return PerceptionState.Outside;
			}

			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				return PerceptionState.BehindWall;
			}
		}
		return PerceptionState.Outside;
	}
	public float GetRandomUpdateInterval()
	{
		return _perceptionData.PerceptionUpdateInterval * Random.Range(0.8f, 1.2f);
	}
}