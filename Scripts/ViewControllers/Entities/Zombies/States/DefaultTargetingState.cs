using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
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
            mTarget.zombieAIController.findPathDirty = true;
            mTarget.ZombieNode.AttackArea.OnTargetStay.Register(OnAttackingAreaStay);
        }

        protected override void OnExit()
        {
            mTarget.ZombieNode.AttackArea.OnTargetStay.UnRegister(OnAttackingAreaStay);
        }

        protected override void OnUpdate()
        {
            // 寻路
            if (mTarget.zombieAIController.findPathDirty)
            {
                // 从当前位置寻路，如果正在爬人梯，从人梯起点开始寻路：否则触发(搭人梯时无法新寻路，导致上面的僵尸很慢才反应过来)的bug（还有一个想法：如果当前位置没有vertex，向下搜寻vertex，给结果加个Fall就行）
                var fromPos = mTarget.zombieAIController.currentMoveData is not { moveType: MoveType.HumanLadder }
                    ? mTarget.CellPos
                    : mTarget.zombieAIController.currentMoveData.from;
                mTarget.zombieAIController.TryFindPath(fromPos);
            }

            // 更新CurrentMoveData
            if (mTarget.zombieAIController.currentMoveData.moveStage == MoveStage.FollowVertex
                && mTarget.CellPos == mTarget.zombieAIController.currentMoveData.target)
            {
                mTarget.zombieAIController.NextTarget();
            }
        }

        protected override void OnFixedUpdate()
        {
            if (mTarget.zombieAIController.currentMoveData != null)
            {
                mTarget.MoveTowards(mTarget.zombieAIController.currentMoveData);
            }
        }


        private void OnAttackingAreaStay(Collider2D other)
        {
            mTarget.AttackingTarget = other.GetComponent<IAttackable>();
            mFSM.ChangeState(ZombieState.Attacking);
        }
    }
}