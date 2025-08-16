using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class ImpZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.ImpZombie;

        public override void OnInit()
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_ImpZombie_Health;
        }

        public override float GetSpeed()
        {
            return base.GetSpeed() * GlobalEntityData.Zombie_ImpZombie_SpeedMultiplier;
        }

        public override AITendency AITendency { get; } = AITendency.Imp;
    }
}