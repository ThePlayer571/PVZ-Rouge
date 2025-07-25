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
            if (!mTarget.effectGroup.ContainsEffectOR(EffectId.Buttered, EffectId.Freeze))
            {
                mFSM.ChangeState(ZombieState.DefaultTargeting);
            }
        }

        protected override void OnExit()
        {
            mTarget.effectGroup.OnEffectRemoved.UnRegister(Soyo);
        }
    }
}