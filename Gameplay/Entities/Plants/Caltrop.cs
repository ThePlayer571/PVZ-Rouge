using System;
using QFramework;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class Caltrop:Plant
    {
        [SerializeField] private AttackDataSO attackDataSO;
        private Attack _attack;
        
        private float _coldTime = Global.peashooterColdTime;
        private float _timer;
        private State _state = State.InCold;
        private BoxCollider2D _collider;

        private enum State
        {
            InCold, Ready
        }

        protected override void DefaultAI()
        {
            if (_state == State.InCold)
            {
                _timer += Time.deltaTime;
                if (_timer >= _coldTime)
                {
                    _timer = 0;
                    _state = State.Ready;
                }
            }
        }

        private void AttackAllInRegion()
        {
            "attackin".LogInfo();  
            var colliders = Physics2D.OverlapBoxAll(_collider.bounds.center, _collider.bounds.size,0,LayerMask.GetMask("Zombie","ZombieShield"));
            foreach (var target in colliders)
            {
                target.gameObject.GetComponent<IAttackable>().TakeDamage(_attack);
            }
        }
        
        protected override void OnAwake()
        {
            _collider = GetComponent<BoxCollider2D>();

            _attack = new Attack(attackDataSO);
            OnCollisionStayEvent += (other) =>
            {
                if (_state == State.Ready)
                {
                    if (other.gameObject.CompareTag("Zombie"))
                    {
                        AttackAllInRegion();
                        // 开始冷却
                        _state = State.InCold;
                    }
                }
            };
        }
    }
}