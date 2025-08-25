using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class BallonZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.BallonZombie;

        [SerializeField] public TriggerDetector BarrierDetector;

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;
        }

        protected override void SetUpFSM()
        {
            "call_1".LogInfo();
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.Flying_BallonZombie, new FlyingState_BallonZombie(FSM, this));
            FSM.AddState(ZombieState.Stunned, new StunnedState(FSM, this));
            FSM.AddState(ZombieState.Dead, new DeadState(FSM, this));

            FSM.StartState(ZombieState.Flying_BallonZombie);
        }
    }
}