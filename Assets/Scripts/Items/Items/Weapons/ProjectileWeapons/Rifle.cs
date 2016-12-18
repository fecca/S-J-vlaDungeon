public class Rifle : ProjectileWeapon
{
	public override string GetName()
	{
		return "Sniper rifle";
	}

	public override float GetWeight()
	{
		return 1.0f;
	}

	public override float GetDamage()
	{
		return 10.0f;
	}

	public override float GetRange()
	{
		return 50.0f;
	}
}