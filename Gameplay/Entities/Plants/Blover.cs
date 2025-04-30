using System;
using System.Collections;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Tags;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using TPL.PVZR.Gameplay.Entities.Zombies;
using TPL.PVZR.Gameplay.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public sealed class Blover : Plant
    {
        
        [SerializeField] private AttackDataSO attackDataSO;
        private Attack _attack;
        
        protected override void OnAwake()
        {
            _attack = new Attack(attackDataSO);
        }

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            // 不能在Awake里面写，不然transform.localScale未初始化
            StartCoroutine(Blow());
        }

        private IEnumerator Blow()
        {
            yield return new WaitForSeconds(0.2f);
            var attack = _attack.WithPunchDirection(direction.ToVector2());
            // 这里也可以遍历EntitySystem里面存储僵尸的数据结构
            var targets = Physics2D.OverlapAreaAll(new Vector2(transform.position.x, -Mathf.Infinity),new Vector2(Mathf.Infinity, Mathf.Infinity), LayerMask.GetMask("Zombie","ZombieShield"));
            foreach (var target in targets)
            {
                var attackable = target.GetComponent<IAttackable>();
                // 移除路障
                if (attackable.tagGroup.Contains(Tag.ConeheadZombie))
                {
                    var _ = target.GetComponent<ArmoredZombie>();
                    if (!_) throw new Exception("带有ConeheadZombie标签的东西不是ArmoredZombie");
                    _.RemoveArmor();
                }
                // Attack
                attackable.TakeDamage(attack);
            }
            Kill();
        }

    }
}