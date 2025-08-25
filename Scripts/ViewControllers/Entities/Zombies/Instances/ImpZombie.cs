using System.Collections.Generic;
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

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_ImpZombie_Health;
            baseSpeed *= GlobalEntityData.Zombie_ImpZombie_BaseSpeedMultiplier;
        }

        public override AITendency aiTendency { get; } = AITendency.Imp;
    }
}