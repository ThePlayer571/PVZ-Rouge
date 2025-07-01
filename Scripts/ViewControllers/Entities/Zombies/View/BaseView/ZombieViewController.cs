using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using TPL.PVZR.ViewControllers.Entities.Zombies.ZombieArmor;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieViewController : MonoBehaviour
    {
        #region 字段

        [SerializeField] private Zombie Zombie;
        [SerializeField] private List<ZombieComponentView> zombieComponentViews;
        [SerializeField] private List<ZombieArmorView> zombieArmorViews;

        #endregion

        private void Awake()
        {
            _Animator = GetComponent<Animator>();

            SetUpFSM();
        }

        private void Start()
        {
            Zombie.OnInitialized.Register(() =>
            {
                Zombie.Direction
                    .RegisterWithInitValue(direction => { Zombie.transform.LocalScaleX(direction.ToInt()); })
                    .UnRegisterWhenGameObjectDestroyed(this);

                // 死亡动画
                Zombie.OnDieFrom.Register(attackData =>
                {
                    foreach (var zombieComponentView in zombieComponentViews)
                    {
                        zombieComponentView.DisassembleWithForce(
                            attackData.Punch(zombieComponentView.transform.position));
                    }

                    foreach (var zombieArmorView in zombieArmorViews.Where(z => z != null))
                    {
                        zombieArmorView.DisassembleWithForce(attackData.Punch(zombieArmorView.transform.position));
                    }
                }).UnRegisterWhenGameObjectDestroyed(this);

                // 盔甲相关
                for (int i = 0; i < Zombie.ZombieArmorList.Count; i++)
                {
                    var target = zombieArmorViews[i];
                    Zombie.ZombieArmorList[i].OnChangeState.Register((armorState, attackData) =>
                        {
                            target.ChangeArmorStateByAttack(armorState, attackData);
                        })
                        .UnRegisterWhenGameObjectDestroyed(this);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private float currentRotation;

        private FSM<ZombieState> ViewFSM;
        private Animator _Animator;
        private float targetRotation;

        private const float rotationChangeFactor = 5f;

        private void SetUpFSM()
        {
            ViewFSM = new FSM<ZombieState>();
            ViewFSM.State(ZombieState.DefaultTargeting)
                .OnEnter(() =>
                {
                    _Animator.SetTrigger(
                        "EnterDefaultTargeting");
                })
                .OnUpdate(() =>
                {
                    targetRotation = Mathf.Clamp(Mathf.Abs(Zombie._Rigidbody2D.velocity.x), 0, 1) * 10f;
                    currentRotation = Mathf.Lerp(currentRotation, targetRotation,
                        Time.deltaTime * rotationChangeFactor);
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
                ViewFSM.ChangeState(Zombie.FSM.CurrentStateId);
            }

            ViewFSM.Update();
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }
    }
}