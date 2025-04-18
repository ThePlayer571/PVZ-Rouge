using QFramework;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class PotatoMine : Plant
    {
        private static readonly int Ready = Animator.StringToHash("Ready");
        [FormerlySerializedAs("attackData")] public AttackDataSO attackDataSO;
        protected Attack attack;

        protected override void Awake()
        {
            base.Awake();
            attack = new Attack(attackDataSO);
            //
            ActionKit.Sequence()
                .Delay(Global.potatoMineSleepTime)
                .Callback(() =>
                {
                    GetComponent<Animator>().SetTrigger(Ready);
                    OnCollisionStayEvent += (other) =>
                    {
                        if (other.collider.CompareTag("Zombie"))
                        {
                            Boom();
                        }
                    };
                }).Start(this);
        }

        protected void Boom()
        {
            var hitAll =
                Physics2D.OverlapCircleAll(transform.position, Global.potatoMineRange,
                    LayerMask.GetMask("Zombie", "ZombieShield"));
            foreach (var hit in hitAll)
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(attack);
            }

            gameObject.DestroySelf();
        }
    }
}