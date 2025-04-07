using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class
{ 
    public struct Attack
    {
        public float damage;
        public float punchForce;
        public Vector2 punchDirection;
        public bool slowness;
        public bool isFrameDamage;

        
        
        
        
        public void Initialize(AttackData data)
        {
            damage = data.damage;
            punchForce = data.punchForce;
            slowness = data.slowness;
        }
        public Attack PunchDirection(Vector2 newPunchDirection)
        {
            var newAttack = this;
            newAttack.punchDirection = newPunchDirection;
            return newAttack;
        }

        public Attack DamageMultiplier(float factor)
        {
            var newAttack = this;
            newAttack.damage *= factor;
            return newAttack;
        }
    }
}