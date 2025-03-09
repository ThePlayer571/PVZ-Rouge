using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntitiyProjectile
{
    public interface IProjectile{}


    public abstract class Projectile : Entity, IProjectile
    {
        // 引用
        protected Rigidbody2D _Rigidbody2D;

        // 数据
        public AttackData attackData;
        protected Attack attack;

        protected override void Awake()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            attack.Initialize(attackData);
            base.Awake();
        }
    }
}