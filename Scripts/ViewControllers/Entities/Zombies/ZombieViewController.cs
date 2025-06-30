using System;
using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieViewController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour zombie;
        private Zombie Zombie;

        private void Awake()
        {
            _Animator = GetComponent<Animator>();

            SetUpFSM();
        }

        private void Start()
        {
            Zombie = zombie.GetComponent<Zombie>();

            Zombie.Direction.RegisterWithInitValue(direction => { Zombie.transform.LocalScaleX(direction.ToInt()); })
                .UnRegisterWhenGameObjectDestroyed(this);
        }
        
        private float currentRotation;

        private FSM<ZombieState> ViewFSM;
        private Animator _Animator;
        private float targetRotation;

        public float factor = 5f;

        private void SetUpFSM()
        {
            ViewFSM = new FSM<ZombieState>();
            ViewFSM.State(ZombieState.DefaultTargeting)
                .OnEnter(() =>
                {
                    _Animator.SetTrigger("EnterDefaultTargeting");
                })
                .OnUpdate(() =>
                {
                    targetRotation = Mathf.Clamp(Mathf.Abs(Zombie._Rigidbody2D.velocity.x), 0, 1) * 10f;
                    currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * factor);
                });
            ViewFSM.State(ZombieState.Attacking)
                .OnEnter(() =>
                {
                    _Animator.SetTrigger("EnterAttacking");
                    currentRotation = 0;
                });
            ViewFSM.StartState(ZombieState.DefaultTargeting);
        }

        private void Update()
        {
            if (ViewFSM.CurrentStateId != Zombie.FSM.CurrentStateId)
            {
                "change state ".LogInfo();
                ViewFSM.ChangeState(Zombie.FSM.CurrentStateId);
            }

            ViewFSM.Update();
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }
    }
}