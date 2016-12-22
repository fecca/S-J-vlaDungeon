using System;

public abstract class ProjectileWeapon : Weapon, IProjectileWeapon
{
	private float _weight;
	private float _damage;
	private float _range;

	protected ProjectileWeapon(ProjectileWeaponTemplate template)
	{
		_weight = (float)Math.Round(UnityEngine.Random.Range(template.MinimumWeight, template.MaximumWeight), 2);
		_damage = (float)Math.Round(UnityEngine.Random.Range(template.MinimumDamage, template.MaximumDamage), 2);
		_range = (float)Math.Round(UnityEngine.Random.Range(template.MinimumRange, template.MaximumRange), 2);
	}

	public override string GetName()
	{
		return GetType().ToString();
	}

	public override float GetWeight()
	{
		return _weight;
	}

	public override float GetDamage()
	{
		return _damage;
	}

	public float GetRange()
	{
		return _range;
	}

	public override string ToString()
	{
		return "[Type:" + GetType() + "] [Name:" + GetName() + "] [Weight:" + GetWeight() + "] [Damage:" + GetDamage() + "] [Range:" + GetRange() + "]";
	}
}