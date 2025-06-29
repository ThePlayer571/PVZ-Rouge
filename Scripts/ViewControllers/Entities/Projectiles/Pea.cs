using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Helpers.Factory;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Pea : Projectile, IPeaLikeInit
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2();
        }

        protected override void Awake()
        {
            base.Awake();

            this.OnCollisionEnter2DEvent.Register(other =>
            {
                if (other.collider.CompareTag("Zombie"))
                {
                    var attackData = AttackHelper.CreateAttackData(AttackId.Pea).WithPunchFrom(transform.position);
                    other.collider.GetComponent<Zombie>().TakeAttack(attackData);
                }

                this.Remove();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}