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
                .OnEnter(() => { _Animator.SetInteger("ZombieState", (int)ZombieState.DefaultTargeting); })
                .OnUpdate(() =>
                {
                    targetRotation = Mathf.Clamp(Mathf.Abs(Zombie._Rigidbody2D.velocity.x), 0, 1) * 10f;
                    currentRotation = Mathf.Lerp(currentRotation, targetRotation,
                        Time.deltaTime * rotationChangeFactor);
                });
            ViewFSM.State(ZombieState.Attacking)
                .OnEnter(() =>
                {
                    _Animator.SetInteger("ZombieState", (int)ZombieState.Attacking);
                    currentRotation = 0;
                });
            ViewFSM.State(ZombieState.OnNewspaperDestroyed)
                .OnEnter(() =>
                {
                    currentRotation = 0;
                    _Animator.SetTrigger("EnterOnNewspaperDestroyed");
                });
            ViewFSM.State(ZombieState.Stunned)
                .OnEnter(() => { _Animator.speed = 0; })
                .OnExit(() => { _Animator.speed = 1; });

            ViewFSM.StartState(ZombieState.DefaultTargeting);
        }

        public void Trigger_OnNewspaperDestroyedPlayFinished()
        {
            if (Zombie.FSM.CurrentState is OnNewspaperDestroyedState current)
            {
                current.Trigger_OnAnimationFinished();
            }
            else
            {
                $"报纸破碎时，FSM State并非OnNewspaperDestroyedState，而是：{Zombie.FSM.CurrentStateId}".LogError();
            }
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