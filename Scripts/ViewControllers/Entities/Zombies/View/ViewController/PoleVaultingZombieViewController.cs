using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.ViewController
{
    /// <summary>
    /// 适用：普通僵尸，路障僵尸，铁桶僵尸，栅栏门僵尸，......
    /// </summary>
    public class PoleVaultingZombieViewController : ZombieViewController
    {
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
            ViewFSM.State(ZombieState.PoleVaulting)
                .OnEnter(() =>
                {
                    _Animator.SetInteger("ZombieState", (int)ZombieState.PoleVaulting);
                    currentRotation = 0;
                });
            ViewFSM.State(ZombieState.Stunned)
                .OnEnter(() => { _Animator.speed = 0; })
                .OnExit(() => { _Animator.speed = 1; });
            ViewFSM.StartState(ZombieState.DefaultTargeting);
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
        }

        protected override void OnUpdate()
        {
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }

        // 命名不佳：应该是起跳时
        public void Trigger_OnTriggerTouchingGround()
        {
            if (Zombie.FSM.CurrentState is PoleVaultingState current)
            {
                current.Trigger_OnPoleTouchGround();
                var pole = zombieComponentViews.First(component => component.gameObject.name == "Pole");
                zombieComponentViews.Remove(pole);
                pole.DisassembleWithForce(Vector2.down);
            }
            else
            {
                $"FSM State并非PoleVaultingState，而是：{Zombie.FSM.CurrentStateId}".LogError();
            }
        }
    }
}