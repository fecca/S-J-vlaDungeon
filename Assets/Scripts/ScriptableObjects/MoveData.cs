public class MoveData
{
	public float PositionUpdateInterval { get; private set; }
	public float MovementSpeed { get; private set; }

	public MoveData(float positionUpdateInterval, float movementSpeed)
	{
		PositionUpdateInterval = positionUpdateInterval;
		MovementSpeed = movementSpeed;
	}
}