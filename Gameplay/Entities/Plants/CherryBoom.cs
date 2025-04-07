using DG.Tweening;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class CherryBoom : Plant
    {
        public AttackData attackData;
        protected Attack attack;
        
        protected override void Awake()
        {
            initialHealthPoint = 11451419;
            base.Awake();
            attack.Initialize(attackData);
            //
        }

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            //
            // 不能在Awake里面写，不然transform.localScale未初始化
            ActionKit.Sequence()
                .Callback(() =>
                {
                    transform.DOScale(transform.localScale * 1.5f, 0.5f).OnComplete(() =>
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