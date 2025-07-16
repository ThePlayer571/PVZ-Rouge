using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public class Spike : Projectile, IPeaLikeInit
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2();
        }

        private List<GameObject> _attacked = new List<GameObject>();

        private void OnCollisionEnter2D(Collision2D other)
        {
            
                if (other.collider.CompareTag("Zombie") && !_attacked.Contains(other.gameObject))
                {
                    _attacked.Add(other.gameObject);
                    var attackData = AttackHelper.CreateAttackData(AttackId.Pea).WithPunchFrom(transform.position);
                    other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
                }

                if (other.collider.IsInLayerMask(LayerMask.GetMask("Barrier")))
                {
                    this.Remove();
                }
        }
    }
}