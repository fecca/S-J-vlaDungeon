public class Data
{
	public HealthData HealthData { get; private set; }
	public AttackData AttackData { get; private set; }
	public MoveData MoveData { get; private set; }
	public PerceptionData PerceptionData { get; private set; }

	public Data(HealthData healthData, AttackData attackData, MoveData moveData, PerceptionData perceptionData)
	{
		HealthData = healthData;
		AttackData = attackData;
		MoveData = moveData;
		PerceptionData = perceptionData;
	}
}