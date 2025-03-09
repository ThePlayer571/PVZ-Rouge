using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityZombie
{
	public partial class ConeheadZombie : Zombie
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
			armor = Conehead;
			base.Awake();
			armor.OnArmorDestroyed.Register(() => { armor = null; });
			armor.OnArmorAttacked.Register((atk) =>
			{
				base.DealAttack(atk);
			});
		}
	}
}
