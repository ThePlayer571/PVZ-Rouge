using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class FirePea : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.FirePea;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Pea_Speed * direction;
        }

        private bool _attacked = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.FirePea)
                    .WithPunchDirectionX(_Rigidbody2D.velocity.x);
                var aoeAttackData = AttackCreator.CreateAttackData(AttackId.FirePeaAOE)
                    .WithPunchFrom(_Rigidbody2D.position);
                
                var targets = Physics2D.OverlapCircleAll(_Rigidbody2D.position,
                    GlobalEntityData.Projectile_FirePea_AOERadius,
                    LayerMask.GetMask("Zombie"));
                foreach (var zombie in targets)
                {
                    zombie.GetComponent<IAttackable>().TakeAttack(new AttackData(aoeAttackData));
                }

                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            Kill();
        }
    }
}