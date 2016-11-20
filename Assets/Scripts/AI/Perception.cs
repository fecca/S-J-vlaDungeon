using UnityEngine;

public class Perception
{
	private PerceptionData _data;

	public Perception()
	{
		_data = ScriptableObject.CreateInstance<PerceptionData>();
	}

	public PlayerPosition GetPerceptionState(Vector3 ownerPosition, Vector3 targetPosition)
	{
		var position = ownerPosition;
		var ray = new Ray(position, (targetPosition - position).normalized);

		var layerMask = (1 << 11) | (1 << 13) | (1 << 14);
		layerMask = ~layerMask;
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
		{
			if (hit.collider.GetComponent<Player>() != null)
			{
				var distance = Vector3.Distance(targetPosition, position);
				if (distance < _data.InnerRadius)
				{
					return PlayerPosition.InnerCirle;
				}

				if (distance < _data.OuterRadius)
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
		return _data.UpdateInterval * Random.Range(0.8f, 1.2f);
	}
}