using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Spike : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.Spike;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Pea_Speed * direction;
        }

        private List<GameObject> _attacked = new List<GameObject>();

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")) && !_attacked.Contains(other.gameObject))
            {
                _attacked.Add(other.gameObject);
                var attackData = AttackCreator.CreateAttackData(AttackId.Spike).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
                if (_attacked.Count >= GlobalEntityData.Projectile_Spike_MaxAttackCount) Kill();
            }

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Barrier")))
            {
                Kill();
            }
        }
    }
}