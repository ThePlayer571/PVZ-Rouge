using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public partial class Pea : Projectile,IPea
    {

        public void Initialize(Direction2 direction)
        {
            if (direction == Direction2.Right)
            {
                _Rigidbody2D.velocity = Vector2.right * Global.peaSpeed;
            }
            else if (direction == Direction2.Left)
            {
                _Rigidbody2D.velocity = Vector2.left * Global.peaSpeed;
            }
        }

        // 变量
        private bool haveAttacked = false; // 本个对象已经造成过攻击
        
        private new void OnCollisionEnter2D(Collision2D collision)
        {
            if (haveAttacked) return;
            haveAttacked = true;

            if (collision.collider.CompareTag("Zombie") || collision.collider.CompareTag("ZombieShield"))
            {
                collision.collider.GetComponent<IDealAttack>()
                    .DealAttack(attack.PunchDirection(
                        (collision.collider.transform.position - this.transform.position)
                        .normalized));
            }

            gameObject.DestroySelf();

        }
    }
}
