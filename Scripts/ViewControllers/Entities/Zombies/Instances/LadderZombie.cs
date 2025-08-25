using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Services;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class LadderZombie : Zombie, IHaveShield
    {
        public override ZombieId Id { get; } = ZombieId.LadderZombie;
        public ICellTileService _CellTileService;

        public ZombieArmorData armorData;

        public override void OnInit(IList<string> paras)
        {
            _CellTileService = this.GetService<ICellTileService>();

            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_LadderZombie_Health;

            armorData = ZombieArmorCreator.CreateZombieArmorData(ZombieArmorId.Ladder);
            ZombieArmorList.Add(armorData);
        }

        public override AITendency aiTendency { get; } = AITendency.CanPutLadder;

        public override float GetSpeed()
        {
            return armorData.IsDestroyed
                ? base.GetSpeed()
                : base.GetSpeed() * GlobalEntityData.Zombie_LadderZombie_WithLadderSpeedMultiplier;
        }

        protected override void SetUpFSM()
        {
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState_LadderZombie(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.Stunned, new StunnedState(FSM, this));
            FSM.AddState(ZombieState.Dead, new DeadState(FSM, this));

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