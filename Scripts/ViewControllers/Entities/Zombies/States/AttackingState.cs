using System;
using QFramework;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class AttackingState : AbstractState<ZombieState, Zombie>
    {
        public AttackingState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            if (mTarget.AttackingTarget == null)
                throw new Exception("切换到AttackingState时，发现AttackingTarget为空");
            mTarget.ZombieNode.AttackArea.OnTargetExit.Register(OnAttackingAreaExit);
        }

        protected override void OnUpdate()
        {
            mTarget.HoldOnLadder();
            mTarget.AttackingTarget.TakeAttack(mTarget.CreateAttackData());
        }

        protected override void OnExit()
        {
            mTarget.ZombieNode.AttackArea.OnTargetExit.UnRegister(OnAttackingAreaExit);
        }

        private void OnAttackingAreaExit(Collider2D other)
        {
            var attackable = other.GetComponent<IAttackable>();
            if (attackable == null) return;

            if (ReferenceEquals(attackable, mTarget.AttackingTarget))
            {
                mTarget.AttackingTarget = null;
                mFSM.ChangeState(ZombieState.DefaultTargeting);
            }
        }
    }
}