using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class FlyingState_BallonZombie : AbstractState<ZombieState, BallonZombie>
    {
        public FlyingState_BallonZombie(FSM<ZombieState> fsm, BallonZombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            mTarget.CachePath = ZombiePath.BallonZombie;
            mTarget.CurrentMoveData = mTarget.CachePath.NextTarget();
        }

        protected override void OnFixedUpdate()
        {
            var targetX = Player.Instance.transform.position.x;
            float distance = Mathf.Abs(mTarget.transform.position.x - targetX);

            // 移动
            if (distance > Global.Zombie_Default_PathFindStopMinDistance)
            {
                this.mTarget.Direction.Value = mTarget.transform.position.x > targetX
                    ? Direction2.Left
                    : Direction2.Right;
                mTarget.MoveForward();
            }

            // 坠落条件
            var case_1 = distance < Global.Zombie_Default_PathFindStopMinDistance;
            var case_2 = mTarget.BarrierDetector.HasTarget;
            if (case_1 || case_2)
            {
                mFSM.ChangeState(ZombieState.DefaultTargeting);
            }
        }

        protected override void OnUpdate()
        {
            "call_update".LogInfo();
        }

        protected override void OnExit()
        {
            mTarget._Rigidbody2D.gravityScale = 1f;
        }
    }
}