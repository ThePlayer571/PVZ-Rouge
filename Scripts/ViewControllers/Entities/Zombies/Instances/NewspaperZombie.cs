using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.Effect;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
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
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            armorData = ZombieArmorCreator.CreateZombieArmorData(ZombieArmorId.Newspaper);
            ZombieArmorList.Add(armorData);
        }

        protected override void SetUpFSM()
        {
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.OnNewspaperDestroyed, new OnNewspaperDestroyedState(FSM, this));
            FSM.AddState(ZombieState.Stunned, new StunnedState(FSM, this));
            FSM.AddState(ZombieState.Dead, new DeadState(FSM, this));

            armorData.OnDestroyed.Register(() =>
            {
                if (!effectGroup.ContainsEffect(EffectId.Freeze))
                    FSM.ChangeState(ZombieState.OnNewspaperDestroyed);
            }).UnRegisterWhenGameObjectDestroyed(this);

            effectGroup.OnEffectAdded.Register(effectData =>
            {
                if (effectData.effectId == EffectId.Freeze)
                {
                    FSM.ChangeState(ZombieState.Stunned);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);

            FSM.StartState(ZombieState.DefaultTargeting);
        }

        public AttackData ShieldTakeAttack(AttackData attackData)
        {
            attackData = armorData.TakeAttack(attackData);
            base.TakeAttack(new AttackData(attackData).OnlyPunch());

            return attackData;
        }

        public ZombieArmorData ShieldArmorData => armorData;
    }
}