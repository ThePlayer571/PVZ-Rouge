using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.ViewControllers.ZombieArmor.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Zombies.Base
{
    public abstract class ArmoredZombie: Zombie
    {
        [SerializeField] protected Armor armor;
        public override void TakeDamage(Attack attack)
        {
            Attack armorLeftAttack = null;
            if (armor)
            {
                armor.TakeDamage(attack, out armorLeftAttack);
            }

            if (armorLeftAttack is not null)
            {
                base.TakeDamage(armorLeftAttack);
            }
        }
    }
}