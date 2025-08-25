using System;
using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class OnNewspaperDestroyedState : AbstractState<ZombieState, Zombie>
    {
        public OnNewspaperDestroyedState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }

        private bool _isFirstEnter = true;

        protected override void OnEnter()
        {
            if (_isFirstEnter)
            {
                mTarget.baseAttackData.MultiplyDamage(4);
                mTarget.baseSpeed *= 3f;

                _isFirstEnter = false;
            }
        }

        public void Trigger_OnAnimationFinished()
        {
            mFSM.ChangeState(ZombieState.DefaultTargeting);
        }
    }
}