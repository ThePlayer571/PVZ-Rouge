using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class FrozenMelon : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.FrozenMelon;

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
                var attackData = AttackCreator.CreateAttackData(AttackId.FrozenMelon)
                    .WithPunchDirection(_Rigidbody2D.velocity);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            var aoeAttackDataTemplate = AttackCreator.CreateAttackData(AttackId.FrozenMelonAOE)
                .WithPunchDirection(_Rigidbody2D.velocity);
            var targets =
                Physics2D.OverlapCircleAll(_Rigidbody2D.position, GlobalEntityData.Projectile_Melon_AOERadius,
                    LayerMask.GetMask("Zombie"));
            foreach (var target in targets)
            {
                var aoeAttackData = new AttackData(aoeAttackDataTemplate);
                target.GetComponent<IAttackable>().TakeAttack(aoeAttackData);
            }

            Kill();
        }
    }
}