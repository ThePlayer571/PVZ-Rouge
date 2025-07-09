using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.ZombieArmor;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class ConeheadZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.ConeheadZombie;

        public ZombieArmorData armorData;

        public override void Initialize()
        {
            base.Initialize();

            baseAttackData = AttackHelper.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            armorData = ZombieArmorHelper.CreateZombieArmorData(ZombieArmorId.TrafficCone);
            ZombieArmorList.Add(armorData);
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            attackData = armorData.TakeAttack(attackData);
            base.TakeAttack(attackData);
            
            return null;
        }
    }
}