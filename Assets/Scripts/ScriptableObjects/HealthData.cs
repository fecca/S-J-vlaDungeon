public class HealthData
{
	public float TotalHealth { get; private set; }
	public float CurrentHealth { get; set; }

	public HealthData(float totalHealth)
	{
		TotalHealth = totalHealth;
		CurrentHealth = totalHealth;
	}
}