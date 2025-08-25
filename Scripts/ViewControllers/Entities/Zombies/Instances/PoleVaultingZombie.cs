using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.Effect;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class PoleVaultingZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.PoleVaultingZombie;

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_PoleVaultingZombie_Health;
        }

        [SerializeField] public TriggerDetector plantDetector;
        [NonSerialized] public bool _hasVaulted = false;
        public EasyEvent OnCollision = new();

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision.Trigger();
        }

        protected override void SetUpFSM()
        {
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState_PoleVaultingZombie(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.PoleVaulting, new PoleVaultingState(FSM, this));
            FSM.AddState(ZombieState.Stunned, new StunnedState(FSM, this));
            FSM.AddState(ZombieState.Dead, new DeadState(FSM, this));

            FSM.StartState(ZombieState.DefaultTargeting);
        }


        public override float GetSpeed()
        {
            var speed = base.GetSpeed();
            if (!_hasVaulted) speed *= GlobalEntityData.Zombie_PoleVaultingZombie_SpeedMultiplier;
            return speed;
        }

        // 操作
        public void Vaulting()
        {
            this._banDrag = true;
            var direction = new Vector2(Direction.Value.ToInt() * 0.7f, 1f);
            var force = direction * GlobalEntityData.Zombie_PoleVaultingZombie_VaultingForce;
            //
            _Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }

        public void VaultingEnd()
        {
            this._banDrag = false;
        }
    }
}