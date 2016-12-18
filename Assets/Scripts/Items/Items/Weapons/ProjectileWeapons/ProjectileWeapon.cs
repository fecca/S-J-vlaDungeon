public abstract class ProjectileWeapon : Weapon, IProjectileWeapon
{
	public abstract float GetRange();

	public override string ToString()
	{
		return "[Type:" + GetType() + "] [Name:" + GetName() + "] [Weight:" + GetWeight() + "] [Damage:" + GetDamage() + "] [Range:" + GetRange() + "]";
	}
}