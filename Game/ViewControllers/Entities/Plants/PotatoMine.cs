using QFramework;
using DG.Tweening;
using UnityEngine;

namespace TPL.PVZR.EntityPlant
{
    public class PotatoMine : Plant
    {
        private static readonly int Ready = Animator.StringToHash("Ready");
        public AttackData attackData;
        protected Attack attack;
        protected override void Awake()
        {
            base.Awake();
            attack.Initialize(attackData);
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
                Physics2D.OverlapCircleAll(transform.position, Global.potatoMineRange, LayerMask.GetMask("Zombie", "ZombieShield"));
            foreach (var hit in hitAll)
            {
                hit.GetComponent<IDealAttack>()?.DealAttack(attack);
            }

            gameObject.DestroySelf();
        }

    }
}