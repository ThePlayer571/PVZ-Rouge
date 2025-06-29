using UnityEngine;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class AttackData
    {
        private float damage;
        private float punchForce;
        private bool isFrameDamage;
        // 我想或许不用让外界访问AttackDefinition，而是提供方便的方法调用


        public void MultiplyDamage(float factor)
        {
            damage *= factor;
        }

        public float Damage
        {
            get
            {
                if (isFrameDamage)
                {
                    return Time.deltaTime * damage;
                }
                else
                {
                    return damage;
                }
            }
        }

        #region 构造函数

        public AttackData(AttackData other)
        {
            this.damage = other.damage;
            this.punchForce = other.punchForce;
            this.isFrameDamage = other.isFrameDamage;
        }

        public AttackData(float damage, float punchForce, bool isFrameDamage)
        {
            this.damage = damage;
            this.punchForce = punchForce;
            this.isFrameDamage = isFrameDamage;
        }

        public AttackData(AttackDefinition attackDefinition)
        {
            this.damage = attackDefinition.damage;
            this.punchForce = attackDefinition.punchForce;
            this.isFrameDamage = attackDefinition.isFrameDamage;
        }

        #endregion
    }
}