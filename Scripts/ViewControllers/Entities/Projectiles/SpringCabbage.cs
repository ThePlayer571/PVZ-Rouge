using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class SpringCabbage : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.SpringCabbage;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Cabbage_Speed * direction;
            _Rigidbody2D.angularVelocity = GlobalEntityData.Projectile_Cabbage_AngularSpeed;
        }

        private bool _attacked = false;
        private int _bounceCount = 0;
        private Vector2 _lastVelocity;

        private void FixedUpdate()
        {
            //todo
            _lastVelocity = _Rigidbody2D.velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.Cabbage)
                    .WithPunchDirection(_lastVelocity);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
                Kill();
            }
            else if (other.collider.IsInLayerMask(LayerMask.GetMask("Barrier")))
            {
                _bounceCount++;
                if (_bounceCount > GlobalEntityData.Projectile_SpringCabbage_MaxBounceCount)
                {
                    Kill();
                }
            }
        }
    }
}