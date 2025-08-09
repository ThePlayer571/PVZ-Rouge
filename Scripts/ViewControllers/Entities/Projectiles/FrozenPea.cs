using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Services;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class FrozenPea : Projectile, IPeaLikeInit, ICanBeIgnited
    {
        public override ProjectileId Id { get; } = ProjectileId.FrozenPea;

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
                var attackData = AttackCreator.CreateAttackData(AttackId.FrozenPea).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            Kill();
        }

        public void Ignite(IgnitionType ignitionType)
        {
            if (_attacked) return;

            switch (ignitionType)
            {
                case IgnitionType.Fire:
                    _attacked = true;
                    _ProjectileService.CreatePea(ProjectileId.Pea, _Rigidbody2D.velocity,
                        _Rigidbody2D.position, this.EntityId);
                    Kill();
                    break;
                case IgnitionType.GhostFire:
                    _attacked = true;
                    _ProjectileService.CreatePea(ProjectileId.GhostFirePea, _Rigidbody2D.velocity,
                        _Rigidbody2D.position, this.EntityId);
                    Kill();
                    break;
                default:
                    break;
            }
        }
    }
}