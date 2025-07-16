using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Pea : Projectile, IPeaLikeInit
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2();
        }

        private bool _attacked = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackHelper.CreateAttackData(AttackId.Pea).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            this.Remove();
        }
    }
}