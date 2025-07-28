using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class DuckyTubeConeheadZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.DuckyTubeConeheadZombie;

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

        public override void SwimForward()
        {
            _Rigidbody2D.AddForce(Direction.Value.ToVector2() *
                                  (this.GetSpeed() * GlobalEntityData.Zombie_DuckyTubeZombie_SwimSpeedMultiplier));
        }

        public override AITendency AITendency { get; } = AITendency.CanSwim;
    }
}