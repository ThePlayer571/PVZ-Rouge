using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityZombie
{
	public partial class BucketZombie : Zombie
	{
		protected Armor armor;

		public override void DealAttack(Attack attack)
		{
			if (armor)
			{
				armor.DealAttack(attack);
			}
			else
			{
				base.DealAttack(attack);
			}
		}

		protected override void Awake()
		{
			armor = Bucket;
			base.Awake();
			armor.OnArmorDestroyed.Register(() => { armor = null; });
			armor.OnArmorAttacked.Register((atk) =>
			{
				base.DealAttack(atk);
			});
		}
	}
}
