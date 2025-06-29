using TPL.PVZR.Helpers.Factory;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class NormalZombie : Zombie
    {
        public override void Initialize()
        {
            base.Initialize();

            baseAttackData = AttackHelper.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;
        }
    }
}