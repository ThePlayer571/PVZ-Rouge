using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Effect;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class StunnedState : AbstractState<ZombieState, Zombie>
    {
        public StunnedState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            mTarget.effectGroup.OnEffectRemoved.Register(Soyo);
        }

        private void Soyo(EffectData effectData)
        {
            if (!mTarget.effectGroup.CanMakeZombieStunned())
            {
                var previous = mFSM.PreviousStateId;
                var next = previous switch
                {
                    ZombieState.DefaultTargeting => ZombieState.DefaultTargeting,
                    ZombieState.Attacking => ZombieState.DefaultTargeting,
                    ZombieState.Stunned => ZombieState.DefaultTargeting,
                    ZombieState.OnNewspaperDestroyed => ZombieState.OnNewspaperDestroyed,
                    _ => throw new ArgumentOutOfRangeException(nameof(previous), previous, null)
                };
                mFSM.ChangeState(next);
            }
        }

        protected override void OnExit()
        {
            mTarget.effectGroup.OnEffectRemoved.UnRegister(Soyo);
        }
    }
}