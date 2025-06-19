using UnityEngine;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class Attack
    {
        public AttackDefinition AttackDefinition;
        // 我想或许不用让外界访问AttackDefinition，而是提供方便的方法调用


        public float Damage
        {
            get
            {
                if (AttackDefinition.isFrameDamage)
                {
                    return Time.deltaTime * AttackDefinition.damage;
                }
                else
                {
                    return AttackDefinition.damage;
                }
            }
        }
    }
}