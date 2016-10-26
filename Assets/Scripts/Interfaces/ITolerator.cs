public interface ITolerator : IBehaviour
{
	bool IsInInnerCirle { get; }
	bool IsInOuterCirle { get; }

	void Detect();
}