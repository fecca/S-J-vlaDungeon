public class RifleTemplate : ProjectileWeaponTemplate
{
	public override float MinimumWeight { get { return 1.8f; } }
	public override float MaximumWeight { get { return 2.2f; } }
	public override float MinimumDamage { get { return 10.0f; } }
	public override float MaximumDamage { get { return 12.0f; } }
	public override float MinimumRange { get { return 40.0f; } }
	public override float MaximumRange { get { return 50.0f; } }
}