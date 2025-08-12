using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class SeasonalBroccoli : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.Seasonal_Broccoli;

        public void Initialize(Vector2 direction)
        {
            var offset = GlobalEntityData.Projectile_SeasonalBroccoli_SpeedOffset;
            var speed = GlobalEntityData.Projectile_Cabbage_Speed + RandomHelper.Default.Range(-offset, offset);
            _Rigidbody2D.velocity = speed * direction;
            _Rigidbody2D.angularVelocity = GlobalEntityData.Projectile_Cabbage_AngularSpeed;
        }

        private bool _attacked = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.Seasonal_Broccoli)
                    .WithPunchDirection(_Rigidbody2D.velocity);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            Kill();
        }
    }
}