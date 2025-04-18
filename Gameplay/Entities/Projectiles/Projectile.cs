using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public interface IProjectile{}


    public abstract class Projectile : Entity, IProjectile
    {
        // 引用
        protected Rigidbody2D _Rigidbody2D;

        // 数据
        [SerializeField] public AttackDataSO attackDataSO;
        protected Attack attack;

        protected override void Awake()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            attack=new Attack(attackDataSO);
            base.Awake();
        }
    }
}