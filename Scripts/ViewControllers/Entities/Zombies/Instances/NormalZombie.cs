using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class NormalZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.NormalZombie;

        public override void Initialize()
        {
            base.Initialize();

            baseAttackData = AttackHelper.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;
        }
    }
}