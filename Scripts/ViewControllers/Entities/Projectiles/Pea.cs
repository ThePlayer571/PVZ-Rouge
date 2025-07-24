using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Pea : Projectile, IPeaLikeInit, ICanBeIgnited
    {
        public override ProjectileId Id { get; } = ProjectileId.Pea;

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
                var attackData = AttackCreator.CreateAttackData(AttackId.Pea).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            this.Remove();
        }

        public void Ignite(IgnitionType ignitionType)
        {
            if (_attacked) return;

            switch (ignitionType)
            {
                case IgnitionType.Fire:
                    _attacked = true;
                    EntityFactory.ProjectileFactory.CreatePea(ProjectileId.FirePea, _Rigidbody2D.velocity,
                        _Rigidbody2D.position);
                    this.Remove();
                    break;
                case IgnitionType.GhostFire:
                    _attacked = true;
                    EntityFactory.ProjectileFactory.CreatePea(ProjectileId.GhostFirePea, _Rigidbody2D.velocity,
                        _Rigidbody2D.position);
                    this.Remove();
                    break;
                default:
                    break;
            }
        }
    }
}