using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.ViewControllers.ZombieArmor.Base;

namespace TPL.PVZR.Gameplay.Entities.Zombies
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
