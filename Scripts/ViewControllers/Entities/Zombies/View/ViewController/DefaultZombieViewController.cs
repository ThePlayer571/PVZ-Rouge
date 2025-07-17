using System.Linq;
using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.ViewController
{
    /// <summary>
    /// 适用：普通僵尸，路障僵尸，铁桶僵尸，栅栏门僵尸，......
    /// </summary>
    public class DefaultZombieViewController : ZombieViewController
    {
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
                    _Animator.SetTrigger("EnterAttacking");
                    currentRotation = 0;
                });
            
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