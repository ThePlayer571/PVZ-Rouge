using QFramework;
using DG.Tweening;
using UnityEngine;

namespace TPL.PVZR.EntityPlant
{
    public class PotatoMine : Plant
    {
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