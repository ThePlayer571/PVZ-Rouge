using System;
using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class PoleVaultingState : AbstractState<ZombieState, PoleVaultingZombie>
    {
        public PoleVaultingState(FSM<ZombieState> fsm, PoleVaultingZombie target) : base(fsm, target)
        {
        }

        private bool _inTheAir = false;
        
        public void Trigger_OnPoleTouchGround()
        {
            mTarget.Vaulting();
            mTarget.OnCollision.Register(Soyo);
        }

        public void Soyo()
        {
            mTarget.VaultingEnd();
            mFSM.ChangeState(ZombieState.DefaultTargeting);
            mTarget.OnCollision.UnRegister(Soyo);
        }
    }
}