using System;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class
{
    public class Attack : ICloneable
    {
        #region 构造函数

        public Attack(float damage = 0, float punchForce = 0, bool isFrameDamage = false, bool slowness = false)
        {
            this.damage = damage;
            this.punchForce = punchForce;
            this.isFrameDamage = isFrameDamage;
            this.slowness = slowness;
        }

        public Attack(AttackDataSO data)
        {
            damage = data.damage;
            punchForce = data.punchForce;
            slowness = data.slowness;
            isFrameDamage = false;
        }

        #endregion

        #region 待配置数据

        public float damage { get; private set; }
        public float punchForce { get; private set; }
        public bool slowness { get; private set; }
        public bool isFrameDamage { get; private set; }
        public float damageValue => isFrameDamage ? damage * Time.deltaTime : damage;

        #endregion

        #region 即时设置数据

        public Vector2 punchDirection { get; private set; }

        public Attack WithPunchDirection(Vector2 newPunchDirection)
        {
            var newAttack = this.Clone() as Attack;
            newAttack.punchDirection = newPunchDirection;
            return newAttack;
        }

        public Attack WithDamage(float newDamage)
        {
            var newAttack = this.Clone() as Attack;
            newAttack.damage = newDamage;
            return newAttack;
        }

        public Attack DamageMultiplier(float factor)
        {
            var newAttack = this.Clone() as Attack;
            newAttack.damage *= factor;
            return newAttack;
        }

        public void SetDamage(float newDamage)
        {
            this.damage = newDamage;
        }

        #endregion


        public object Clone()
        {
            return new Attack()
                { damage = damage, punchForce = punchForce, slowness = slowness, isFrameDamage = isFrameDamage };
        }
    }
}