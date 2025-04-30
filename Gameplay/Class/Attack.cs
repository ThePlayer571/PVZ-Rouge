using System;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class
{
    public class Attack : ICloneable
    {
        #region 构造函数

        public Attack(float damage = 0, float punchForce = 0, bool isFrameDamage = false, bool slowness = false, bool butter = false)
        {
            this.damage = damage;
            this.punchForce = punchForce;
            this.isFrameDamage = isFrameDamage;
            this.slowness = slowness;
            this.butter = butter;
        }

        public Attack(AttackDataSO data)
        {
            damage = data.damage;
            punchForce = data.punchForce;
            slowness = data.slowness;
            this.butter = data.buttered;
            isFrameDamage = data.isFrameDamage;
        }

        #endregion

        #region 待配置数据

        // 不推荐直接调用
        public float damage { get; private set; }

        public bool isFrameDamage { get; private set; }

        // 推荐直接调用
        public float punchForce { get; private set; }
        public bool slowness { get; private set; }
        public bool butter { get; private set; }
        public float damageValue => isFrameDamage ? damage * Time.deltaTime : damage;

        #endregion

        #region 即时设置数据

        public Vector2 punchDirection { get; private set; }

        /// <summary>
        /// 返回一个新的Attack，修改其PunchDirection（不修改原Attack的值）
        /// </summary>
        /// <param name="newPunchDirection"> 新的击退方向(不会自动normalize)</param>
        /// <returns></returns>
        public Attack WithPunchDirection(Vector2 newPunchDirection)
        {
            var newAttack = this.Clone() as Attack;
            newAttack.punchDirection = newPunchDirection;
            return newAttack;
        }

        public Attack WithPunchDirection(Vector2 from, Vector2 to)
        {
            return this.WithPunchDirection((to - from).normalized);
        }

        public Attack WithDamage(float newDamage)
        {
            var newAttack = this.Clone() as Attack;
            newAttack.damage = newDamage;
            return newAttack;
        }

        /// <summary>
        /// （不会修改原Attack的值）
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Attack WithDamageMultiplier(float factor)
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