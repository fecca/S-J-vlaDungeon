public interface IBrain
{
	IPathFinderAgent Agent { get; }
	void InitializePathfindingAgent();
	void InitializeAttacker(AttackData attackData);
	void InitializeMover(MoveData moveData);
	void Think();
	void ClearOccupiedAgentNodes();
}