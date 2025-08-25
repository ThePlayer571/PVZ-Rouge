using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class SeasonalTomato : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.Seasonal_Tomato;

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
                // 单体
                var attackData = AttackCreator.CreateAttackData(AttackId.Seasonal_Tomato)
                    .WithPunchDirection(_Rigidbody2D.velocity);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            // AOE
            var aoeAttackDataTemplate = AttackCreator.CreateAttackData(AttackId.Seasonal_TomatoAOE)
                .WithPunchFrom(_Rigidbody2D.position);
            var targets = Physics2D.OverlapCircleAll(_Rigidbody2D.position,
                GlobalEntityData.Projectile_Melon_AOERadius, LayerMask.GetMask("Zombie"));
            foreach (var target in targets)
            {
                target.GetComponent<IAttackable>().TakeAttack(new AttackData(aoeAttackDataTemplate));
            }

            Kill();
        }
    }
}