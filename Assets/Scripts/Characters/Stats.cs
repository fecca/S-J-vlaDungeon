public class Stats
{
	public float TotalHitpoints { get; private set; }
	public float CurrentHealth { get; set; }

	public Stats(HealthData healthData)
	{
		TotalHitpoints = healthData.Health;
		CurrentHealth = TotalHitpoints;
	}
}