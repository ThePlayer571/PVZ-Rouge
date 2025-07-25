using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Kernel : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.Kernel;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Cabbage_Speed * direction;
            _Rigidbody2D.angularVelocity = GlobalEntityData.Projectile_Cabbage_AngularSpeed;
        }

        private bool _attacked = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.Kernel)
                    .WithPunchDirection(_Rigidbody2D.velocity);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            this.Remove();
        }
    }
}