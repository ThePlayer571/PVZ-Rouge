using QFramework;
using DG.Tweening;
using UnityEngine;

namespace TPL.PVZR.EntityPlant
{
    public class CherryBoom : Plant
    {
        public AttackData attackData;
        protected Attack attack;
        
        protected override void Awake()
        {
            base.Awake();
            attack.Initialize(attackData);
            //
            ActionKit.Sequence()
                .Callback(() =>
                {
                    transform.DOScale(1.5f, 0.5f).OnComplete(() =>
                    {
                        Boom();
                    });
                }).Start(this);
        }

        protected void Boom()
        {
            var hitAll =
                Physics2D.OverlapCircleAll(transform.position, Global.cherryBoomRange, LayerMask.GetMask("Zombie", "ZombieShield"));
            foreach (var hit in hitAll)
            {
                hit.GetComponent<IDealAttack>()?.DealAttack(attack);
            }
            Kill();
        }

    }
}