using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class ConeheadZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.ConeheadZombie;

        public ZombieArmorData armorData;

        public override void OnInit()
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            armorData = ZombieArmorCreator.CreateZombieArmorData(ZombieArmorId.TrafficCone);
            ZombieArmorList.Add(armorData);
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            if (!armorData.IsDestroyed) attackData = armorData.TakeAttack(attackData);
            base.TakeAttack(attackData);

            return null;
        }
    }
}