using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public class Spike : Projectile, IPea
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody2D.velocity = direction.ToVector2() * Global.peaSpeed;
        }

        // 变量
        private List<GameObject> _EnemiesHaveAttacked = new();

        private new void OnCollisionEnter2D(Collision2D collision)
        {
            // 碰到僵尸/防具
            if (collision.collider.CompareTag("Zombie") || collision.collider.CompareTag("ZombieShield"))
            {
                if (_EnemiesHaveAttacked.Contains(collision.collider.gameObject)) return;
                
                collision.collider.GetComponent<IAttackable>()
                    .TakeDamage(attack.WithPunchDirection(
                        (collision.collider.transform.position - this.transform.position)
                        .normalized));
                _EnemiesHaveAttacked.Add(collision.collider.gameObject);
            }

            // 碰到墙体
            if (collision.collider.IsInLayerMask(LayerMask.GetMask("Barrier")))
            {
                gameObject.DestroySelf();
            }
        }
    }
}