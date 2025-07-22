using System.Linq;
using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.ViewController
{
    public sealed class NewspaperZombieViewController : ZombieViewController
    {
        /// <summary>
        /// 适用：仅读报僵尸
        /// </summary>
        private float currentRotation;

        private float targetRotation;
        private const float rotationChangeFactor = 5f;

        protected override void SetUpFSM()
        {
            ViewFSM.State(ZombieState.DefaultTargeting)
                .OnEnter(() => { _Animator.SetTrigger("EnterDefaultTargeting"); })
                .OnUpdate(() =>
                {
                    targetRotation = Mathf.Clamp(Mathf.Abs(Zombie._Rigidbody2D.velocity.x), 0, 1) * 10f;
                    currentRotation = Mathf.Lerp(currentRotation, targetRotation,
                        Time.deltaTime * rotationChangeFactor);
                });
            ViewFSM.State(ZombieState.Attacking)
                .OnEnter(() =>
                {
                    currentRotation = 0;
                    _Animator.SetTrigger("EnterAttacking");
                });
            ViewFSM.State(ZombieState.OnNewspaperDestroyed)
                .OnEnter(() =>
                {
                    currentRotation = 0;
                    _Animator.SetTrigger("EnterOnNewspaperDestroyed");
                });
            ViewFSM.State(ZombieState.Frozen)
                .OnEnter(() =>
                {
                    _Animator.SetTrigger("EnterFrozen");
                    currentRotation = 0;
                });

            ViewFSM.StartState(ZombieState.DefaultTargeting);
        }

        // todo 屎山：让动画控制器决定逻辑
        public void Shit_OnNewspaperDestroyedPlayFinished()
        {
            Zombie.FSM.ChangeState(ZombieState.DefaultTargeting);
        }

        protected override void OnInit()
        {
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
        }

        protected override void OnUpdate()
        {
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }
    }
}