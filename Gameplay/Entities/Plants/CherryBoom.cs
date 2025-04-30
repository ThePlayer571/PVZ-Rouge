using DG.Tweening;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class CherryBoom : Plant
    {
        [SerializeField] public AttackDataSO attackDataSO;
        protected Attack attack;
        
        protected override void OnAwake()
        {
            attack = new Attack(attackDataSO);
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
            "call boom".LogInfo();
            var hitAll =
                Physics2D.OverlapCircleAll(transform.position, Global.cherryBoomRange, LayerMask.GetMask("Zombie", "ZombieShield"));
            foreach (var hit in hitAll)
            {
                hit.GetComponent<IAttackable>()?.TakeDamage(attack);
            }
            Kill();
        }

    }
}