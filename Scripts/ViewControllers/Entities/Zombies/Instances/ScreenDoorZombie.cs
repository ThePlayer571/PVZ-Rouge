using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.ZombieArmor;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class ScreenDoorZombie : Zombie, IHaveShield
    {
        public override ZombieId Id { get; } = ZombieId.ScreenDoorZombie;

        public ZombieArmorData armorData;

        public override void OnInit()
        {
            baseAttackData = AttackHelper.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            armorData = ZombieArmorHelper.CreateZombieArmorData(ZombieArmorId.ScreenDoor);
            ZombieArmorList.Add(armorData);
        }

        public AttackData ShieldTakeAttack(AttackData attackData)
        {
            attackData = armorData.TakeAttack(attackData);
            base.TakeAttack(attackData.OnlyPunch());

            return null;
        }
    }
}