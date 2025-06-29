using System;
using QFramework;
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
            mTarget.AttackArea.OnTargetExit.Register(OnAttackingAreaExit);
        }

        protected override void OnUpdate()
        {
            mTarget.AttackingTarget.TakeAttack(mTarget.CreateAttackData());
        }

        protected override void OnExit()
        {
            mTarget.AttackArea.OnTargetExit.UnRegister(OnAttackingAreaExit);
        }

        private void OnAttackingAreaExit(Collider2D other)
        {
            if (ReferenceEquals(other, mTarget.AttackingTarget))
            {
                mTarget.AttackingTarget = null;
                mFSM.ChangeState(ZombieState.DefaultTargeting);
            }
        }
    }
}