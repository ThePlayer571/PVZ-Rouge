using UnityEngine;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class AttackData
    {
        private float damage;
        private float punchForce;
        private bool isFrameDamage;
        // 我想或许不用让外界访问AttackDefinition，而是提供方便的方法调用


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
    }
}