using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class FirePea : Projectile, IPeaLikeInit
    {
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
                var attackData = AttackCreator.CreateAttackData(AttackId.FirePea).WithPunchFrom(transform.position);
                var aoeAttackData = AttackCreator.CreateAttackData(AttackId.FirePeaAOE)
                    .WithPunchFrom(transform.position);
                var targets = Physics2D.OverlapCircleAll(_Rigidbody2D.position, GlobalEntityData.Projectile_FirePea_AOERadius,
                    LayerMask.GetMask("Zombie"));
                foreach (var zombie in targets)
                {
                    zombie.GetComponent<IAttackable>().TakeAttack(new AttackData(aoeAttackData));
                }
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            this.Remove();
        }
    }
}