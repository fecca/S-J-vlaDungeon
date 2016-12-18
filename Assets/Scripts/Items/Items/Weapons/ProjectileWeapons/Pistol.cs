public class Pistol : ProjectileWeapon
{
	public override string GetName()
	{
		return "Magnum 356";
	}

	public override float GetWeight()
	{
		return 0.5f;
	}

	public override float GetDamage()
	{
		return 6.0f;
	}

	public override float GetRange()
	{
		return 20.0f;
	}
}