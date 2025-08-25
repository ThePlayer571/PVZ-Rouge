using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class NormalZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.NormalZombie;

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;
        }
    }
}