using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
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
            mTarget._timeToFindPath = true;
            mTarget.AttackArea.OnTargetStay.Register(OnAttackingAreaStay);
        }

        protected override void OnExit()
        {
            mTarget.AttackArea.OnTargetStay.UnRegister(OnAttackingAreaStay);
        }

        protected override void OnUpdate()
        {
            // 
            bool allowRefind = mTarget._ZombieAISystem.ZombieAIUnit.GetVertexSafely(mTarget.CellPos) != null;
            if (allowRefind && mTarget._timeToFindPath)
            {
                mTarget._timeToFindPath = false;
                mTarget.FindPath(mTarget._ZombieAISystem.PlayerVertexPos);
            }

            // 更新CurrentMoveData
            if (mTarget.CurrentMoveData.moveStage == MoveStage.FollowVertex)
            {
                if (mTarget.CellPos == mTarget.CurrentMoveData.target)
                {
                    mTarget.CurrentMoveData = mTarget.CachePath.NextTarget();
                    $"新的目标点: {mTarget.CurrentMoveData.target}, {mTarget.CurrentMoveData.moveType}, {mTarget.CurrentMoveData.moveStage}"
                        .LogInfo();
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