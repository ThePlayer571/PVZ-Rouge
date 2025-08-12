using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class OakArcherArrow : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.OakArcherArrow;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_SnipePea_Speed * direction;
            attackData = AttackCreator.CreateAttackData(AttackId.OakArcherArrow);
        }

        private bool _attackDepleted = false;
        private AttackData attackData;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attackDepleted) return;
            
            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                attackData =
                    other.collider.GetComponent<IAttackable>()
                        .TakeAttack(attackData.WithPunchFrom(_Rigidbody2D.position));
            }
            else if (other.collider.IsInLayerMask(LayerMask.GetMask("Barrier")))
            {
                Kill();
            }

            if (attackData.Damage <= 0)
            {
                _attackDepleted = true;
                Kill();
            }
        }
    }
}