using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class DefaultTargetingState_PoleVaultingZombie : DefaultTargetingState
    {
        public DefaultTargetingState_PoleVaultingZombie(FSM<ZombieState> fsm, PoleVaultingZombie target) : base(fsm,
            target)
        {
            mTarget = target;
        }

        private new PoleVaultingZombie mTarget;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            //
            if (!mTarget._hasVaulted && mTarget.plantDetector.HasTarget)
            {
                mFSM.ChangeState(ZombieState.PoleVaulting);
                mTarget._hasVaulted = true;
            }
        }
    }
}