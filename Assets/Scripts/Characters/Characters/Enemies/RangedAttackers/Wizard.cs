public class _Enemy
{
}
public class RangedEnemy : _Enemy
{
	public RangedEnemy(RangedEnemyTemplate rangedEnemyTemplate)
	{

	}
}
public class Wizard : RangedEnemy
{
	public Wizard(WizardTemplate wizardTemplate) : base(wizardTemplate) { }
}

public class EnemyTemplate
{
}
public class RangedEnemyTemplate : EnemyTemplate
{

}
public class WizardTemplate : RangedEnemyTemplate
{

}