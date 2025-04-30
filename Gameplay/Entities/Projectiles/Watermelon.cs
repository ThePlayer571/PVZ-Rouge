using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public sealed class Watermelon : Projectile, ICabbage
    {

        [SerializeField] private AttackDataSO scatterAttackData;
        private Attack scatterAttack;
        [SerializeField] private CircleCollider2D ScatterRange;
        public void Initialize(Direction2 direction)
        {
            _Rigidbody2D.velocity = new Vector2(direction.ToVector2().x/2, 0.866f) * Global.cabbageCultThrowPower;
        }

        protected override void OnAwake()
        {
            scatterAttack = new Attack(scatterAttackData);
        }

        // 变量
        private bool haveAttacked = false; // 本个对象已经造成过攻击

        private new void OnCollisionEnter2D(Collision2D collision)
        {
            if (haveAttacked) return;
            haveAttacked = true;

            if (collision.collider.CompareTag("Zombie") || collision.collider.CompareTag("ZombieShield"))
            {
                collision.collider.GetComponent<IAttackable>()
                    .TakeDamage(attack.WithPunchDirection(
                        this.transform.position, collision.collider.transform.position));
                // 溅射
               var colliders =  Physics2D.OverlapCircleAll(ScatterRange.transform.position, ScatterRange.radius);
               foreach (var collider in colliders)
               {
                   if (collider.CompareTag("Zombie") || collider.CompareTag("ZombieShield"))
                   {
                       collider.GetComponent<IAttackable>().TakeDamage(scatterAttack);
                   }
               }
            }

            gameObject.DestroySelf();
        }
    }
}