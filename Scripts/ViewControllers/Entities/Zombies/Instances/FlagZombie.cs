using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class FlagZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.FlagZombie;

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            this.baseSpeed *= GlobalEntityData.Zombie_FlagZombie_BaseSpeedMultiplier;
        }
    }
}