using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.ViewController
{
    /// <summary>
    /// 仅适用：气球僵尸
    /// </summary>
    public class BalloonZombieViewController : ZombieViewController
    {
        [SerializeField] private SpriteRenderer Balloon;
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
            ViewFSM.State(ZombieState.Flying_BallonZombie)
                .OnEnter(() =>
                {
                    _Animator.SetInteger("ZombieState", (int)ZombieState.Flying_BallonZombie);
                })
                .OnExit(() =>
                {
                    Balloon.enabled = false;
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
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void OnUpdate()
        {
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }
    }
}