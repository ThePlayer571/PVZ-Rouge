using QFramework;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class ExcavatingState_DiggerZombie : AbstractState<ZombieState, DiggerZombie>
    {
        public ExcavatingState_DiggerZombie(FSM<ZombieState> fsm, DiggerZombie target) : base(fsm, target)
        {
        }

        public void Trigger_OnAnimationEnd()
        {
            mFSM.ChangeState(ZombieState.DefaultTargeting);
        }

        protected override void OnEnter()
        {
            mTarget._collider.enabled = true;
        }

        protected override void OnExit()
        {
            mTarget.GetComponent<Collider2D>().enabled = true;
            mTarget._Rigidbody2D.gravityScale = 1f;
        }
    }
}