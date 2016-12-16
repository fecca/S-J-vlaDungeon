public interface IBrain
{
	void InitializeAttacker(AttackData attackData, IAttacker attacker);
	void InitializeMover(MoveData moveData, IMover mover);
	void Think();
	void SmoothStop();
}