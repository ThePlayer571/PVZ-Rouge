using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class DefaultTargetingState_LadderZombie : DefaultTargetingState
    {
        public DefaultTargetingState_LadderZombie(FSM<ZombieState> fsm, LadderZombie target) : base(fsm,
            target)
        {
            mTarget = target;
        }

        private new LadderZombie mTarget;
        private float _lastLadderPutTime = -1f;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            // 尝试搭梯
            if (!mTarget.armorData.IsDestroyed)
            {
                if (mTarget.zombieAIController.currentMoveData.moveType == MoveType.HumanLadder)
                {
                    var cellPos = mTarget.CellPos;
                    if (cellPos.y < mTarget.zombieAIController.currentMoveData.target.y)
                    {
                        var _ = mTarget._CellTileService.TryPutLadder(cellPos);
                        if (_)
                            _lastLadderPutTime = Time.time;
                    }
                }

                // 重新烘焙
                if (mTarget.zombieAIController.currentMoveData.moveType != MoveType.HumanLadder &&
                    _lastLadderPutTime > mTarget._ZombieAISystem.ZombieAIUnit.LastBakeTime)
                {
                    mTarget._ZombieAISystem.ZombieAIUnit.RebakeDirty = true;
                }
            }
        }
    }
}