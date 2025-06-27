using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.ViewControllers.Entities.Plants;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class DefaultTargetingState : AbstractState<ZombieState, Zombie>
    {
        public DefaultTargetingState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            mTarget.AttackArea.OnTargetStay.Register(OnAttackingAreaStay);
        }

        protected override void OnExit()
        {
            mTarget.AttackArea.OnTargetStay.UnRegister(OnAttackingAreaStay);
        }

        protected override void OnUpdate()
        {
            mTarget.Direction = (mTarget.transform.position.x > ReferenceHelper.Player.transform.position.x)
                ? Direction2.Left
                : Direction2.Right;
            mTarget.MoveForward();
        }

        private void OnAttackingAreaStay(Collider2D other)
        {
            mTarget.AttackingTarget = other.GetComponent<IAttackable>();
            mFSM.ChangeState(ZombieState.Attacking);
        }
    }
}