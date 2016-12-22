public class PerceptionData
{
	public float PerceptionUpdateInterval { get; private set; }
	public float InnerRadius { get; private set; }
	public float OuterRadius { get; private set; }

	public PerceptionData(float perceptionUpdateInterval, float innerRadius, float outerRadius)
	{
		PerceptionUpdateInterval = perceptionUpdateInterval;
		InnerRadius = innerRadius;
		OuterRadius = outerRadius;
	}
}