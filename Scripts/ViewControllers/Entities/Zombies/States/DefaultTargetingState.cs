using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
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
            // mTarget.AttackArea.OnTargetStay.Register(OnAttackingAreaStay);
        }

        protected override void OnExit()
        {
            mTarget.AttackArea.OnTargetStay.UnRegister(OnAttackingAreaStay);
        }

        protected override void OnUpdate()
        {
            // 手动触发重新寻路
            if (mTarget.triggerDebug)
            {
                mTarget.CachePath = mTarget._ZombieAISystem.ZombieAIUnit.FindPath(mTarget.CellPos,
                    ReferenceHelper.Player.CellPos, mTarget.AITendency);
                mTarget.triggerDebug = false;
                mTarget.CurrentMoveData = mTarget.CachePath.NextTarget();
            }

            if (mTarget.CurrentMoveData == null) return;

            // 更新CurrentMoveData
            if (mTarget.CurrentMoveData.moveStage == MoveStage.FollowVertex)
            {
                if (mTarget.CellPos == mTarget.CurrentMoveData.target)
                {
                    mTarget.CurrentMoveData = mTarget.CachePath.NextTarget();
                }
            }

            //
            mTarget.MoveTowards(mTarget.CurrentMoveData);
        }


        private void OnAttackingAreaStay(Collider2D other)
        {
            mTarget.AttackingTarget = other.GetComponent<IAttackable>();
            mFSM.ChangeState(ZombieState.Attacking);
        }
    }
}