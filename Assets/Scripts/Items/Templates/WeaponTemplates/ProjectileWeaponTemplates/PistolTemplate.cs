public class PistolTemplate : ProjectileWeaponTemplate
{
	public override float MinimumWeight { get { return 0.2f; } }
	public override float MaximumWeight { get { return 0.4f; } }
	public override float MinimumDamage { get { return 6.0f; } }
	public override float MaximumDamage { get { return 8.0f; } }
	public override float MinimumRange { get { return 20.0f; } }
	public override float MaximumRange { get { return 30.0f; } }
}