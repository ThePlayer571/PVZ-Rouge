using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Services;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class FrostSpike : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.FrostSpike;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_SnipePea_Speed * direction;
        }

        private bool _attacked = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.FrostSpike).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            Kill();
        }
    }
}