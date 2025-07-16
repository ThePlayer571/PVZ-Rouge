using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.ZombieArmor;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class NewspaperZombie : Zombie, IHaveShield
    {
        public override ZombieId Id { get; } = ZombieId.NewspaperZombie;

        public ZombieArmorData armorData;
        
        

        public override void OnInit()
        {
            baseAttackData = AttackHelper.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            armorData = ZombieArmorHelper.CreateZombieArmorData(ZombieArmorId.Newspaper);
            ZombieArmorList.Add(armorData);

            armorData.OnDestroyed.Register(() =>
                FSM.ChangeState(ZombieState.OnNewspaperDestroyed)
            ).UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void SetUpFSM()
        {
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.OnNewspaperDestroyed,new OnNewspaperDestroyed(FSM, this));

            FSM.StartState(ZombieState.DefaultTargeting);
        }

        public AttackData ShieldTakeAttack(AttackData attackData)
        {
            attackData = armorData.TakeAttack(attackData);
            base.TakeAttack(attackData.OnlyPunch());

            return null;
        }
    }
}