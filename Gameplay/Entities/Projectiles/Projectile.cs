using System;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public interface IProjectile : IEntity
    {
        
    }


    public abstract class Projectile : Entity, IProjectile
    {
        // 引用
        protected Rigidbody2D _Rigidbody2D;

        // 数据
        [SerializeField] public AttackDataSO attackDataSO;
        protected Attack attack;

        protected override void OnAwakeBase()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            attack=new Attack(attackDataSO);
        }

        public override void Kill()
        {
            throw new NotImplementedException();
        }
    }
}