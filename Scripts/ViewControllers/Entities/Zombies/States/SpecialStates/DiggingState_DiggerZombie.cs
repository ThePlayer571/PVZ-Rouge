using QFramework;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class DiggingState_DiggerZombie : AbstractState<ZombieState, DiggerZombie>
    {
        public DiggingState_DiggerZombie(FSM<ZombieState> fsm, DiggerZombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            mTarget._collider.enabled = false;
            mTarget._Rigidbody2D.gravityScale = 0;
        }

        protected override void OnFixedUpdate()
        {
            var targetX = mTarget._excavatePosX;
            float distance = Mathf.Abs(mTarget.transform.position.x - targetX);

            // 移动
            if (distance > Global.Zombie_Default_PathFindStopMinDistance)
            {
                this.mTarget.Direction.Value = mTarget.transform.position.x > targetX
                    ? Direction2.Left
                    : Direction2.Right;
                mTarget.MoveForward();
            }
            
            // 出土条件
            var case_1 = distance < Global.Zombie_Default_PathFindStopMinDistance;
            if (case_1)
            {
                mFSM.ChangeState(ZombieState.Excavating_DiggerZombie);
            }
        }
    }
}